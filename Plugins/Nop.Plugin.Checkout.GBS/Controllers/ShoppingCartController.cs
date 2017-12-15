using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Controllers;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Media;
using Nop.Services.Tax;
using Nop.Services.Directory;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Shipping;
using Nop.Services.Security;
using Nop.Services.Messages;
using Nop.Services.Common;
using Nop.Core.Caching;
using System.Web;
using Nop.Services.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework.Security.Captcha;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Web.Models.ShoppingCart;
using System.Globalization;
using Nop.Web.Framework.Security;
using Nop.Web;
using Nop.Core.Plugins;
using Nop.Plugin.Checkout.GBS;
using Newtonsoft.Json;
using Nop.Web.Framework.Mvc;
using Nop.Services.Shipping.Date;
using Nop.Web.Factories;
using System.Reflection;
using Nop.Core.Infrastructure;
using System.Data;


namespace Nop.Plugin.ShoppingCart.GBS.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
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
            ICacheManager cacheManager,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            HttpContextBase httpContext,
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
            CustomerSettings customerSettings) : base(
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

        }

        #endregion

        [ChildActionOnly]
        public ActionResult GBSOrderTotals(bool isEditable)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var shoppingCartModel = new ShoppingCartModel();
            _shoppingCartModelFactory.PrepareShoppingCartModel(shoppingCartModel, cart,
            isEditable: false,
            prepareEstimateShippingIfEnabled: true,
            prepareAndDisplayOrderReviewData: false);
            ViewBag.ShoppingCartModel = shoppingCartModel;

            var model = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable);
            return PartialView("OrderTotals", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ActionName("MyPDA")]
        new public ActionResult ProductDetails_AttributeChange(int productId, bool validateAttributeConditions,
                    bool loadPicture, FormCollection form)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                JsonResult result = base.ProductDetails_AttributeChange(productId, validateAttributeConditions, loadPicture, form) as JsonResult;
                var json = JsonConvert.SerializeObject(result.Data);
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
                        q = Int32.Parse(System.Web.HttpContext.Current.Request.Form[key]);
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

                    decimal finalPrice = decimal.Parse(dresult.price.Value, NumberStyles.Currency| NumberStyles.AllowThousands|NumberStyles.AllowDecimalPoint);  

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
                    dresult.price = "$"+finalPrice;
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
        [ValidateInput(false)]
        public override ActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, FormCollection form)
        {
            var groupId = 0000;
            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            Product product = productService.GetProductById(productId);
            ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            var specAttrs = specService.GetProductSpecificationAttributes(productId);

            //IList<ProductSpecificationAttribute> list = specService.GetProductSpecificationAttributes(productId);
                  
            foreach (var item in specAttrs)
            {
                if(item.SpecificationAttributeOption.SpecificationAttribute.Name == "AccessoryGroup")
                {
                    groupId = Int32.Parse(item.SpecificationAttributeOption.Name);
                }
            }

            JsonResult action = (JsonResult)base.AddProductToCart_Details(productId, shoppingCartTypeId, form);

            if (groupId != 0000)
            {               

                Type t = action.Data.GetType();
                PropertyInfo redirect = t.GetProperty("redirect");
                PropertyInfo success = t.GetProperty("success");
                string redirectValue = redirect != null ? (string)redirect.GetValue(action.Data) : null;
                bool successValue = success != null ? (bool)success.GetValue(action.Data) : false;

                if ((redirectValue != null && redirectValue == "/cart") || successValue)
                {
                    return Json(new
                    {
                        redirect = Url.RouteUrl("AccessoryPage", new { groupId = groupId, productId = productId }),
                    });
                }
            }
                        
            return action;
            
        }

        //Add to cart without leaving page while using amalgamation pricing on galleries
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProductToCart_Amalgamation(int productId, int shoppingCartTypeId, int quantity)
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
            else{
                base.AddProductToCart_Catalog(productId, shoppingCartTypeId, quantity);
            }

            return Json(new
            {
                qty = quantity,
            });

        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AmalgamationBarUpdate(int categoryId, int featuredProductId)
        {
           
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            IProductService productService = EngineContext.Current.Resolve<IProductService>();

            int totalCartons = 0;
            decimal cartTotalPrice = 0.00M;
            decimal eachPrice = 0;
            string amountToNextTier = "";
            string tierNextEach = "";
            int unitsPerCarton = 100; // will need to be dynamic at some point

            //total cartons
            ICollection<ShoppingCartItem> shoppingCart = _workContext.CurrentCustomer.ShoppingCartItems;
            foreach (ShoppingCartItem item in shoppingCart)
            {
                IList<ProductCategory> productsCategories = categoryService.GetProductCategoriesByProductId(item.Product.Id);

                foreach (var category in productsCategories)
                {
                    if(category.Category.Id == categoryId)
                    {
                        totalCartons += item.Quantity;
                        break; 
                    }
                }
                
            }

            //total price
            Product featuredProduct = productService.GetProductById(featuredProductId);
            ICollection<TierPrice> tiers = featuredProduct.TierPrices;
            int t = 0;
            TierPrice tierForPrice = null;
            TierPrice tierForNextDiscount = null;
            
            foreach (var tier in tiers)
            {
                                
                if(totalCartons >= tier.Quantity)
                {
                    tierForPrice = tier;

                    if(tiers.Count == t) //customer is getting final tier pricing, best price
                    {
                        tierForNextDiscount = tier;
                        break;
                    }
                }
                else
                {
                    if(tierForPrice != null && tierForNextDiscount == null)
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
            if(tierForPrice != null && tierForNextDiscount != null)
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

            });

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AmalgamationCartCategoryTotal(int categoryId, int productId)
        {

            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            int cartTotal = 0;
            int singleTotal = 0;

            ICollection<ShoppingCartItem> shoppingCart = _workContext.CurrentCustomer.ShoppingCartItems;
            foreach (ShoppingCartItem item in shoppingCart)
            {
                IList<ProductCategory> productsCategories = categoryService.GetProductCategoriesByProductId(item.Product.Id);

                if(item.Product.Id == productId)
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
        
    }
}
