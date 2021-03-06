using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Checkout.GBS;
using Nop.Plugin.Checkout.GBS.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using static Nop.Plugin.Order.GBS.Orders.OrderExtensions;

namespace Nop.Plugin.ShoppingCart.GBS.Controllers
{
    public class GBSShoppingCartController : Nop.Web.Controllers.ShoppingCartController
    {

        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISettingService _settingService;
        private readonly IDownloadService _downloadService;
        private readonly ILogger _logger;
        private readonly IShoppingCartService _shoppingCartService;

        #endregion

        #region Constructors

        public GBSShoppingCartController(
            IShoppingCartModelFactory shoppingCartModelFactory,
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,
            ICustomerService customerService,
            IGiftCardService giftCardService,
            IDateRangeService dateRangeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService,
            IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            IPluginFinder pluginFinder,
            ISettingService settingService,
            CustomerSettings customerSettings,
            ILogger logger) : base(
                  shoppingCartModelFactory,
                    productService,
                    storeContext,
                    workContext,
                    shoppingCartService,
                    pictureService,
                    localizationService,
                    productAttributeService,
                    productAttributeParser,
                    taxService,
                    currencyService,
                    priceCalculationService,
                    priceFormatter,
                    checkoutAttributeParser,
                    discountService,
                    customerService,
                    giftCardService,
                    dateRangeService,
                    checkoutAttributeService,
                    workflowMessageService,
                    permissionService,
                    downloadService,
                    cacheManager,
                    webHelper,
                    customerActivityService,
                    genericAttributeService,
                    mediaSettings,
                    shoppingCartSettings,
                    orderSettings,
                    captchaSettings,
                    customerSettings
                )
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pluginFinder = pluginFinder;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._priceFormatter = priceFormatter;
            this._downloadService = downloadService;
            this._productAttributeService = productAttributeService;
            this._settingService = settingService;
            this._logger = logger;

        }

		#endregion

		public IActionResult GBSOrderTotals(bool isEditable)
		{
			var cart = _workContext.CurrentCustomer.ShoppingCartItems
				.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
				.LimitPerStore(_storeContext.CurrentStore.Id)
				.ToList();

			var shoppingCartModel = new ShoppingCartModel();
			_shoppingCartModelFactory.PrepareShoppingCartModel(shoppingCartModel, cart,
			isEditable: false,
			prepareAndDisplayOrderReviewData: false);
			ViewBag.ShoppingCartModel = shoppingCartModel;

			var model = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable);
			return PartialView("OrderTotals", model);
		}

		[HttpPost]
        [ActionName("MyPDA")]
        new public IActionResult ProductDetails_AttributeChange(int productId, bool validateAttributeConditions,
                    bool loadPicture, IFormCollection form)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                JsonResult result = base.ProductDetails_AttributeChange(productId, validateAttributeConditions, loadPicture, form) as JsonResult;
                var json = JsonConvert.SerializeObject(result.Value);
                dynamic dresult = JsonConvert.DeserializeObject<dynamic>(json);
                var product = _productService.GetProductById(productId);
                var errors = new List<string>();
                if (product == null)
                    return new NullJsonResult();
                string attributesXml = ParseProductAttributes(product, form, errors);

                bool hasReturnAddressAttr = false;
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
                decimal returnAddressPriceAdjustment = 0;
                //get quantity from form
                int q = 1;
                foreach (string key in form.Keys)
                {
                    if (key.Contains("EnteredQuantity"))
                    {
                        q = Int32.Parse(HttpContext.Request.Form[key]);
                    }
                }
                //int returnAddressMinimumSurchargeID = 0;
                //bool returnAddressMinimumSurchargeApplied = true;
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        if (attributeValue.ProductAttributeMapping.ProductAttribute.Name == "Add Return Address" && attributeValue.Name == "Yes")
                        {
                            hasReturnAddressAttr = true;
                            returnAddressPriceAdjustment = attributeValue.PriceAdjustment;
                            //if the quantity < 100 then make adjustment equal to the $5/quantity and round since it is being rounded in the price calculation service override
                            if (q < 100)
                            {
                                returnAddressPriceAdjustment = RoundingHelper.RoundPrice(5 / (decimal)q);
                            }

                        }

                        //if (attributeValue.ProductAttributeMapping.ProductAttribute.Name == "returnAddressMinimumSurcharge" && attributeValue.Name == "Yes")
                        //{
                        //    returnAddressMinimumSurchargeID = attributeValue.Id;
                        //    returnAddressMinimumSurchargeApplied = true;
                        //}
                        //else
                        //{
                        //    returnAddressMinimumSurchargeApplied = false;
                        //}
                    }
                }
                if (hasReturnAddressAttr)
                {

                    decimal finalPrice = decimal.Parse(dresult.price.Value, NumberStyles.Currency | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);

                    if (q < 100)
                    {
                        //redo calculaton to get an unrounded amount so that the CC editor can calculate correctly.
                        finalPrice -= returnAddressPriceAdjustment;
                        //eventually make this configurable
                        decimal adj = (5 / (decimal)q);
                        finalPrice += adj;
                    }
                    else
                    {
                        ////remove returnAddressMinimumSurcharge if still on
                        //if (returnAddressMinimumSurchargeApplied)
                        //{
                        //    //   _productAttributeParser.ParseProductAttributeMappings(attributesXml);
                        //}

                    }
                    dresult.price = "$" + finalPrice;
                    //put it back in the result
                    json = JsonConvert.SerializeObject(dresult);

                    var newResult = new ContentResult()
                    {
                        Content = json,
                        ContentType = "application/json"
                    };
                    return newResult;
                }

            }
            //if we got here just return the base value;
            return base.ProductDetails_AttributeChange(productId, validateAttributeConditions, loadPicture, form);
        }

        [HttpPost]
        public override IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, IFormCollection form)
        {
            var groupId = 0000;
            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            Product product = productService.GetProductById(productId);

            //ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            //var specAttrs = specService.GetProductSpecificationAttributes(productId);

            ////IList<ProductSpecificationAttribute> list = specService.GetProductSpecificationAttributes(productId);

            //foreach (var item in specAttrs)
            //{
            //    if(item.SpecificationAttributeOption.SpecificationAttribute.Name == "AccessoryGroup")
            //    {
            //        groupId = Int32.Parse(item.SpecificationAttributeOption.Name);
            //    }
            //}

            JsonResult action = (JsonResult)base.AddProductToCart_Details(productId, shoppingCartTypeId, form);

            //if (groupId != 0000)
            //{               


            Type t = action.Value.GetType();
            PropertyInfo redirect = t.GetProperty("redirect");
            PropertyInfo success = t.GetProperty("success");
            string redirectValue = redirect != null ? (string)redirect.GetValue(action.Value) : null;
            bool successValue = success != null ? (bool)success.GetValue(action.Value) : false;

            if ((redirectValue != null && redirectValue == "/cart") || successValue)
            {


                action = (JsonResult)CheckForAccessories(action, product);

                //        return Json(new
                //        {
                //            redirect = Url.RouteUrl("AccessoryPage", new { groupId = groupId, productId = productId }),
                //        });

            }
            //}

            return action;

        }

        public IActionResult CheckForAccessories(IActionResult action, Product product)
        {
            var groupId = 0000;
            ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            var specAttrs = specService.GetProductSpecificationAttributes(product.Id);

            //IList<ProductSpecificationAttribute> list = specService.GetProductSpecificationAttributes(productId);

            foreach (var item in specAttrs)
            {
                if (item.SpecificationAttributeOption.SpecificationAttribute.Name == "AccessoryGroup")
                {
                    groupId = Int32.Parse(item.SpecificationAttributeOption.Name);
                }
            }


            if (groupId != 0000)
            {

                //Type t = action.Data.GetType();
                //PropertyInfo redirect = t.GetProperty("redirect");
                //PropertyInfo success = t.GetProperty("success");
                //string redirectValue = redirect != null ? (string)redirect.GetValue(action.Data) : null;
                //bool successValue = success != null ? (bool)success.GetValue(action.Data) : false;

                //if ((redirectValue != null && redirectValue == "/cart") || successValue)
                //{
                return Json(new
                {
                    redirect = Url.RouteUrl("AccessoryPage", new { groupId = groupId, productId = product.Id }),
                });
                //}
            }

            return action;

        }

        //Add to cart without leaving page while using amalgamation pricing on galleries
        [HttpPost]
        public IActionResult AddProductToCart_Amalgamation(int productId, int shoppingCartTypeId, int quantity)
        {
            //check if item in cart already
            ICollection<ShoppingCartItem> shoppingCart = _workContext.CurrentCustomer.ShoppingCartItems;
            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            Product product = productService.GetProductById(productId);
            ShoppingCartItem item = _shoppingCartService.FindShoppingCartItemInTheCart(shoppingCart.ToList(), ShoppingCartType.ShoppingCart, product);


            if (item != null)
            {
                if (quantity == 0)
                {
                    _shoppingCartService.DeleteShoppingCartItem(item);
                }
                else
                {
                    _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer, item.Id, item.AttributesXml, item.CustomerEnteredPrice, null, null, quantity);
                }
            }
            else
            {
                base.AddProductToCart_Catalog(productId, shoppingCartTypeId, quantity);
            }

            return Json(new
            {
                qty = quantity,
            });

        }

        [HttpPost]
        public IActionResult AmalgamationBarUpdate(int categoryId, int featuredProductId)
        {

            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();

            int totalCartons = 0;
            decimal cartTotalPrice = 0.00M;
            decimal eachPrice = 0;
            string amountToNextTier = "";
            string tierNextEach = "";
            int unitsPerCarton = 9999; //not fully implemented, needs spec attr
            string packType = "Carton";

            //total cartons
            ICollection<ShoppingCartItem> shoppingCart = _workContext.CurrentCustomer.ShoppingCartItems;
            List<ShoppingCartItem> AmalgamationList = new List<ShoppingCartItem>();

            foreach (var item in shoppingCart)
            {
                var specAttrs = specService.GetProductSpecificationAttributes(item.ProductId);
                foreach (var spec in specAttrs)
                {
                    string type = "";
                    if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                    {
                        type = spec.SpecificationAttributeOption.Name;
                        if (type == "Carton")
                        {
                            //cartItemList.Remove(item);
                            AmalgamationList.Add(item);
                        }
                    }
                    //get carton units per
                    if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Quantity Per Carton")
                    {
                        unitsPerCarton = Int32.Parse(spec.SpecificationAttributeOption.Name);
                    }
                }
            }
            //check if units per value was found else set a default of 100
            unitsPerCarton = unitsPerCarton == 9999 ? 100 : unitsPerCarton;

            //use featured products pack type
            var featuredSpecAttrs = specService.GetProductSpecificationAttributes(featuredProductId);
            foreach (var spec in featuredSpecAttrs)
            {
                if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                {
                    packType = spec.SpecificationAttributeOption.Name;
                }

            }

            foreach (ShoppingCartItem item in AmalgamationList)
            {
                IList<ProductCategory> productsCategories = categoryService.GetProductCategoriesByProductId(item.Product.Id);

                foreach (var category in productsCategories)
                {
                    if (category.Category.Id == categoryId)
                    {
                        totalCartons += item.Quantity;
                        break;
                    }
                }

            }

            //total price
            Product featuredProduct = productService.GetProductById(featuredProductId);
            ICollection<TierPrice> tiers = featuredProduct.TierPrices;
            int t = 1;
            TierPrice tierForPrice = null;
            TierPrice tierForNextDiscount = null;

            foreach (var tier in tiers)
            {

                if (totalCartons >= tier.Quantity)
                {
                    tierForPrice = tier;

                    if (tiers.Count == t) //customer is getting final tier pricing, best price
                    {
                        tierForNextDiscount = tier;
                        //break;
                    }
                }
                else
                {
                    if (tierForPrice != null && tierForNextDiscount == null)
                    {
                        tierForNextDiscount = tier;
                    }
                    else
                    {
                        //default featured price will be used
                    }

                    break;
                }

                t++;
            }
            //qty to next tier
            if (tierForPrice != null && tierForNextDiscount != null)
            {
                amountToNextTier = tierForPrice == tierForNextDiscount ? "best" : (tierForNextDiscount.Quantity - totalCartons).ToString();

                //each price
                tierNextEach = amountToNextTier != "best" ? (tierForNextDiscount.Price / unitsPerCarton).ToString("#.#0") : "best";
            }

            //check if qty is not high enough for tier pricing
            cartTotalPrice = tierForPrice != null ? totalCartons * tierForPrice.Price : totalCartons * featuredProduct.Price;

            //each price
            eachPrice = tierForPrice != null ? tierForPrice.Price / unitsPerCarton : featuredProduct.Price / unitsPerCarton;

            return Json(new
            {
                totalCartons = totalCartons,
                cartTotalPrice = cartTotalPrice.ToString("0.#0"),
                eachPrice = eachPrice.ToString("#.#0"),
                tierNext = amountToNextTier,
                tierNextEach = tierNextEach,
                packType = packType,

            });

        }

        [HttpPost]
        public IActionResult AmalgamationCartCategoryTotal(int categoryId, int productId)
        {

            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();

            int cartTotal = 0;
            int singleTotal = 0;

            ICollection<ShoppingCartItem> shoppingCart = _workContext.CurrentCustomer.ShoppingCartItems;
            List<ShoppingCartItem> AmalgamationList = new List<ShoppingCartItem>();
            foreach (var item in shoppingCart)
            {
                var specAttrs = specService.GetProductSpecificationAttributes(item.ProductId);
                foreach (var spec in specAttrs)
                {
                    string type = "";
                    if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                    {
                        type = spec.SpecificationAttributeOption.Name;
                        if (type == "Carton")
                        {
                            //cartItemList.Remove(item);
                            AmalgamationList.Add(item);
                        }
                    }
                }
            }

            foreach (ShoppingCartItem item in AmalgamationList)
            {
                IList<ProductCategory> productsCategories = categoryService.GetProductCategoriesByProductId(item.Product.Id);

                if (item.Product.Id == productId)
                {
                    singleTotal = item.Quantity;
                }

                foreach (var category in productsCategories)
                {
                    if (category.Category.Id == categoryId)
                    {
                        cartTotal += item.Quantity;
                        break;
                    }
                }

            }



            return Json(new
            {
                cartTotal = cartTotal,
                singleTotal = singleTotal,
            });

        }

        #region Custom Submit Cart For Edit
        [HttpPost]
        public IActionResult SetProductOptions(string options)//int productId, string dataJson, int quantity, string optionsJson = "", string formOptions = "")
        {
            var productMappings = _productAttributeService.GetProductAttributeMappingsByProductId(2993);

            HttpContext.Session.SetString("productOptions", options.ToString());
            return Json(new { status = "success" });
        }

        [HttpPost]
        public IActionResult SubmitItem(string productId, string dataJson, string quantity, string cartImageSrc, string editActive = "", string formOptions = "", string cartItemId = "", string productXml = "")//, int cartItemId = 0)
        {
            var customer = _workContext.CurrentCustomer;
            IList<string> warnings = new List<string>();
            try
            {

                var product = _productService.GetProductById(Convert.ToInt32(productId));


                string optionsAttributesXml = string.Empty;
                if (Convert.ToBoolean(editActive.ToLower()))
                {
                    _shoppingCartService.UpdateShoppingCartItem(customer, Convert.ToInt32(cartItemId), "", 0, quantity: 0);
                    optionsAttributesXml = productXml;
                }
                else
                {
                    var prodOptions = HttpContext.Session.GetString("productOptions") == null ? "" :  HttpContext.Session.GetString("productOptions");
                    var options = DeserializeOptions(prodOptions);
                    optionsAttributesXml = GetOptionsAttributes(product, options);
                }
                var attributesXml = GetProductAttributes(product, formOptions, dataJson, cartImageSrc, optionsAttributesXml);
                warnings = _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, attributesXml, quantity: Convert.ToInt32(quantity));

                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                var orderItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, ShoppingCartType.ShoppingCart, product, attributesXml);

                JsonResult action = (JsonResult)CheckForAccessories(null, product);


                if (warnings.Count > 0)
                {
                    throw new Exception("Add to cart failed");
                }
                SubmitItemResponse response = new SubmitItemResponse();
                response.status = "success";

                if (action != null)
                {
                    return action;
                }
                else
                {
                    return Json(new
                    {
                        redirect = "/cart"
                    });

                }//////////////////////////////////////////////////

            }
            catch (Exception ex)
            {
                _logger.Error("Error in SubmitItem = productId = " + productId, ex, customer);
                SubmitItemResponse response = new SubmitItemResponse();
                response.status = "failure";
                response.message = ex.Message;
                response.warnings = warnings;
                return Json(response);
            }

        }


        private Dictionary<string, SelectedOption> DeserializeOptions(string optionsJson)
        {
            var result = new Dictionary<string, SelectedOption>();
            if (!string.IsNullOrEmpty(optionsJson))
            {
                var converter = new OptionValueConverter();
                result = JsonConvert.DeserializeObject<Dictionary<string, SelectedOption>>(optionsJson, converter);
            }
            return result;
        }



        private string GetProductAttributes(Core.Domain.Catalog.Product product, string formOptions, string iframeData, string cartImageSrc, string optionsAttributesXml)
        {
            var productMappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);

            int iframeDataId = 0;
            int customImgId = 0;
            foreach (var item in productMappings)
            {
                switch (item.ProductAttribute.Name)
                {
                    case "IframeData":
                        iframeDataId = item.Id;
                        break;
                    case "CustomImgUrl":
                        customImgId = item.Id;
                        break;
                    default:
                        break;
                }


            }
            string customImgXml = string.Format("<ProductAttribute ID=\"{0}\"><ProductAttributeValue><Value>{1}</Value></ProductAttributeValue></ProductAttribute>", customImgId, cartImageSrc);

            var formOptionsAttributeXml = GetFormOptionsAttributesXml(product, formOptions);
            optionsAttributesXml += formOptionsAttributeXml;

            return string.Format("<Attributes>" +
                                 "<ProductAttribute ID=\"{0}\"><ProductAttributeValue><Value>{1}</Value></ProductAttributeValue></ProductAttribute>" +
                                 optionsAttributesXml +
                                 customImgXml +
                                 "</Attributes>", iframeDataId.ToString(), iframeData);

        }


        private string GetOptionsAttributes(Core.Domain.Catalog.Product product, Dictionary<string, SelectedOption> options)
        {
            var result = new StringBuilder();
            foreach (var option in options)
            {
                var val = option.Value;
                var attrId = val.Option.Id;

                if (val.Value != null && val.Value.Any())
                {
                    result.AppendFormat("<ProductAttribute ID=\"{0}\">", attrId);
                    foreach (var selectedOption in val.Value)
                    {
                        result.AppendFormat("<ProductAttributeValue><Value>{0}</Value></ProductAttributeValue>", selectedOption);
                    }
                    result.AppendLine("</ProductAttribute>");
                }
            }

            return result.ToString();
        }


        private string GetFormOptionsAttributesXml(Product product, string formOptions)
        {
            Dictionary<string, StringValues> nameValueCollection = new Dictionary<string, StringValues>();
            var jsonFormOptions = string.IsNullOrEmpty(formOptions) ? null : JObject.Parse(formOptions);

            if (jsonFormOptions != null && jsonFormOptions.Root != null)
            {
                var root = jsonFormOptions.Root;
                if (root.First != null)
                {
                    var item = root.First;
                    while (item != null)
                    {
                        var name = item.Path;
                        var value = item.First.Value<string>();
                        nameValueCollection.Add(name, value);
                        item = item.Next;
                    }
                    var formCollection = new FormCollection(nameValueCollection);
                    var attributesXml = ParseProductAttributes(product, formCollection);
                    attributesXml = attributesXml.Replace("<Attributes>", "").Replace("</Attributes>", "");
                    return attributesXml;
                }
            }
            return "";
        }

        private string ParseProductAttributes(Product product, IFormCollection form)
        {
            string attributesXml = "";

            #region Product attributes

            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.Keys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }
        #endregion
        [HttpGet]
        public IActionResult BuyItAgain(int orderItemId, bool isLegacy)
        {
            var customer = _workContext.CurrentCustomer;
            IList<string> warnings = new List<string>();
            JsonResult responseJSON = Json(new SubmitItemResponse());
            OrderItem orderItem = new OrderItem();
            SubmitItemResponse response = new SubmitItemResponse();
            try
            {
                var cart = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                orderItem = new Nop.Plugin.Order.GBS.Orders.OrderExtensions().GetOrderItemById(orderItemId, isLegacy);

                if (isLegacy)
                {
                    var legacyOrderItem = (LegacyOrderItem)orderItem;
                    //build the dataJson object.  need to add it to the stored proc?  select for json into a json array inside an AttributesXml doc?
                    responseJSON = (JsonResult)SubmitItem(legacyOrderItem.ProductId.ToString(), legacyOrderItem.productOptionsJson, legacyOrderItem.Quantity.ToString(), legacyOrderItem.legacyPicturePath, "false", "", "", "");
                }
                else
                {
                    warnings = _shoppingCartService.AddToCart(customer, orderItem.Product, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, orderItem.AttributesXml, quantity: Convert.ToInt32(orderItem.Quantity));
                }

                response = (SubmitItemResponse)responseJSON.Value;
                if (response.status != "success" || warnings.Count > 0)
                {
                    throw new Exception("Failed to submit item to cart");
                }
                return RedirectToRoute("ShoppingCart");
            }
            catch (Exception ex)
            {
                var finalWarnings = warnings;
                if (warnings.Count == 0)
                {
                    finalWarnings = response.warnings;
                }
                _logger.Error("Error in Buy it Again = orderitemid = " + orderItemId + ", warnings = " + JsonConvert.SerializeObject(finalWarnings), ex, customer);
                string productURL = Url.RouteUrl("Product", new { SeName = orderItem.Product.GetSeName() });
                if (string.IsNullOrEmpty(productURL))
                {
                    return RedirectToRoute("ShoppingCart");
                }
                else
                {
                    return Redirect(productURL);

                }
            }
        }


        [HttpPost]
        public IActionResult NameBadgeIframeAddToCart(IFormCollection form)
        {
            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            IProductAttributeService productAttributeService = EngineContext.Current.Resolve<IProductAttributeService>();
            IProductAttributeParser productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
            Customer customer = _workContext.CurrentCustomer;

            string productIds = form["accChk"];
            string[] idsArray = productIds.Split(',');
            string productCodes = form["productCode"];
            string[] codesArray = productCodes.Split(',');
            //string frameValue = "";
            //string frameId = "";
            int frameOptionValueId = 0;
            int frameQty = 0;
            //string attributesXml = "";

            string redirect = "";
            string productIdForAccessoryCheck = "";

            foreach (var productIdIFrame in idsArray)
            {
                string checkId = form["accQTY" + productIdIFrame];
                //string frameId = form["rp" + productIdIFrame + "option"];
                //int frameOptionValueId = 0;
                string customerName = "";
                string customerTitle = "";
                string companyName = "";
                string attributesXml = "";
                string backStyle = "";
                ProductAttributeMapping frameStyleMappy = null;

                if (!string.IsNullOrEmpty(checkId) && checkId != "0")
                {
                    Product product = null;

                    foreach (var codeCombo in codesArray)
                    {
                        if (codeCombo.Contains(productIdIFrame + "|"))
                        {
                            string[] productIdCodeSplit = codeCombo.Split('|');
                            //create to get nop product id
                            product = productService.GetProductBySku(productIdCodeSplit[1]);

                            //var combinationValues = productAttributeService.GetAllProductAttributeCombinations(product.Id);

                            //get attribute mapping
                            ICollection<ProductAttributeMapping> productAttributes = product.ProductAttributeMappings;
                            //ProductAttributeMapping frameStyleMappy = null;


                            //ProductAttributeMapping pam = new ProductAttributeMapping();

                            //Collection<ProductAttributeMapping> productAttributes2 = new Collection<ProductAttributeMapping>();
                            //product.ProductAttributeMappings = productAttributes2;

                            //foreach (ProductAttributeCombination combinationValue in combinationValues)
                            //{

                            foreach (var attr in productAttributes)
                            {
                                if (attr.ProductAttribute.Name == "Frame Style")
                                {

                                    //productAttributeService.GetProductAttributeValueById(attr.ProductAttributeId);
                                    //foreach (ProductAttributeValue val in attr.ProductAttributeValues)
                                    //{
                                    //    if (val.Name == GetFrameStyleValue(frameId))
                                    //    {
                                    //        frameOptionValueId = val.Id;
                                    //    }
                                    //}

                                    frameStyleMappy = attr;
                                }

                                if (string.IsNullOrEmpty(backStyle) && attr.ProductAttribute.Name == "Back")
                                {
                                    backStyle = form["text357"];
                                    if (!string.IsNullOrEmpty(form["text357"].ToString()))
                                    {
                                        attributesXml = productAttributeParser.AddProductAttribute(attributesXml, attr, backStyle);
                                    }
                                }

                                if (string.IsNullOrEmpty(companyName) && attr.ProductAttribute.Name == "Company Name")
                                {
                                    companyName = form["text281"];
                                    if (!string.IsNullOrEmpty(form["text281"].ToString()))
                                    {
                                        attributesXml = productAttributeParser.AddProductAttribute(attributesXml, attr, companyName);
                                    }

                                }

                                if (string.IsNullOrEmpty(customerName) && attr.ProductAttribute.Name == "Customer Name")
                                {
                                    customerName = form["text279"];
                                    if (!string.IsNullOrEmpty(form["text279"].ToString()))
                                    {
                                        attributesXml = productAttributeParser.AddProductAttribute(attributesXml, attr, customerName);
                                    }
                                }

                                if (string.IsNullOrEmpty(customerTitle) && attr.ProductAttribute.Name == "Customer Title")
                                {
                                    if (!string.IsNullOrEmpty(form["chooseTitle"].ToString()))
                                    {
                                        customerTitle = form["chooseTitle"];
                                        attributesXml = productAttributeParser.AddProductAttribute(attributesXml, attr, customerTitle);
                                    }
                                    else if (!string.IsNullOrEmpty(form["customTitle"].ToString()))
                                    {
                                        customerTitle = form["customTitle"];
                                        attributesXml = productAttributeParser.AddProductAttribute(attributesXml, attr, customerTitle);
                                    }
                                    else
                                    {
                                        //dont add customer title
                                    }
                                }

                            }

                            //}

                            //TODO determine which attribute option has been selected for frame color
                            string frameId = form["rp" + productIdIFrame + "option"];

                            //TODO
                            //check if rp[productCode]option  is for specific badge type or if option is 0

                            //if only badge type, add to cart with qty
                            //if 0, then mix type was selected. check all badge types for qty and add each with qty as seperate products, 
                            if (frameId == "0")
                            {
                                //mix frames selected
                                //will add each variant frame style so must loop thorugh all possible styles
                                string[] frameStyles = new string[] { "359", "360", "361", "362", "455" };
                                foreach (var frameStyle in frameStyles)
                                {
                                    string mixNameBadgeAttributesXml = "";
                                    int frameOptionValueIdMix = 0;

                                    //check if style of frame has quantity to add to cart
                                    if (Int32.Parse(form["rp" + productIdIFrame + "option" + frameStyle + "qty"]) > 0)
                                    {
                                        //generate attr xml
                                        //frameValue = GetFrameStyleValue(frameStyle);

                                        productAttributeService.GetProductAttributeValueById(frameStyleMappy.ProductAttributeId);
                                        foreach (ProductAttributeValue val in frameStyleMappy.ProductAttributeValues)
                                        {
                                            if (val.Name == GetFrameStyleValue(frameStyle))
                                            {
                                                frameOptionValueIdMix = val.Id;
                                            }
                                        }

                                        frameQty = Int32.Parse(form["rp" + productIdIFrame + "option" + frameStyle + "qty"]);
                                        //each mix frame name badge needs xml with empty frame style attr, else they append to same cart item
                                        mixNameBadgeAttributesXml = productAttributeParser.AddProductAttribute(attributesXml, frameStyleMappy, frameOptionValueIdMix.ToString());

                                        //add to cart
                                        _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, mixNameBadgeAttributesXml, quantity: frameQty);

                                    }
                                }

                            }
                            else
                            {
                                //adding only a single frame style
                                string frameValue = GetFrameStyleValue(frameId);
                                frameQty = Int32.Parse(form["rp" + productIdIFrame + "option" + frameId + "qty"]);

                                //for testing bad values
                                //frameValue = "Spider Monkey";

                                productAttributeService.GetProductAttributeValueById(frameStyleMappy.ProductAttributeId);
                                foreach (ProductAttributeValue val in frameStyleMappy.ProductAttributeValues)
                                {
                                    if (val.Name == frameValue)
                                    {
                                        frameOptionValueId = val.Id;
                                        break;
                                    }
                                }

                                //add new value to xml with mapping AddProductAttribute
                                attributesXml = productAttributeParser.AddProductAttribute(attributesXml, frameStyleMappy, frameOptionValueId.ToString());

                                //add to cart
                                _shoppingCartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, attributesXml, quantity: frameQty);

                            }

                            break;

                        }
                    }

                }

            }

            string[] productAccessory = codesArray[0].Split('|');
            productIdForAccessoryCheck = productAccessory[1];

            Product accessoryProduct = productService.GetProductBySku(productIdForAccessoryCheck);

            IActionResult action = null;
            JsonResult redirectJson = (JsonResult)CheckForAccessories(action, accessoryProduct);

            if (redirectJson == null)
            {
                redirect = Url.RouteUrl("ShoppingCart");
            }
            else
            {
                Type t = redirectJson.Value.GetType();
                PropertyInfo redirectData = t.GetProperty("redirect");
                string redirectLink = (string)redirectData.GetValue(redirectJson.Value);
                redirect = redirectLink;
            }

            return Json(new
            {
                redirect = redirect,
            });
        }

        private string GetFrameStyleValue(string frameId)
        {
            string frameValue = "";

            switch (frameId)
            {

                case "359":
                    frameValue = "Black";
                    //frameQty = Int32.Parse(form["rp" + productId + "option" + frameId + "qty"]);
                    return frameValue;

                case "362":
                    frameValue = "Silver";
                    //frameQty = Int32.Parse(form["rp" + productId + "option" + frameId + "qty"]);
                    return frameValue;

                case "361":
                    frameValue = "Gold";
                    //frameQty = Int32.Parse(form["rp" + productId + "option" + frameId + "qty"]);
                    return frameValue;

                case "360":
                    frameValue = "Bling";
                    //frameQty = Int32.Parse(form["rp" + productId + "option" + frameId + "qty"]);
                    return frameValue;

                case "455":
                    frameValue = "Frameless";
                    //frameQty = Int32.Parse(form["rp" + productId + "option" + frameId + "qty"]);
                    return frameValue;

                default:
                    break;

            }

            return frameValue;

        }


    }

}