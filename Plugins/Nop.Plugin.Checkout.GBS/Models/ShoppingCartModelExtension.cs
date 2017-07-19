using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Payments;
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
using Nop.Core.Plugins;
using Nop.Web.Models.ShoppingCart;
using Nop.Core.Infrastructure;
using Nop.Web.Factories;
using Nop.Services.Shipping.Date;

namespace Nop.Plugin.Checkout.GBS.Models
{
    public class ShoppingCartModelExtension : ShoppingCartController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly HttpContextBase _httpContext;

        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        #endregion

        #region Constructors

        public ShoppingCartModelExtension(
            IShoppingCartModelFactory shoppingCartModelFactory,
            IPluginFinder pluginFinder,
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService,
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
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._giftCardService = giftCardService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._paymentService = paymentService;
            this._workflowMessageService = workflowMessageService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._httpContext = httpContext;

            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;
            this._shoppingCartModelFactory = shoppingCartModelFactory;

        }
        #endregion

        public ShoppingCartModel GetCurrentShoppingCartModel(ShoppingCartModel model, List<ShoppingCartItem> cart)
        {
           
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
            return model;
        }
    }
}
