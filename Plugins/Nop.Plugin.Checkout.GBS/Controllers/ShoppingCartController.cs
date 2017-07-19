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
    }
}
