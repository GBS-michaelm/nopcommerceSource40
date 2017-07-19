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
using Nop.Plugin.Checkout.GBS.Controllers;
using Nop.Plugin.Checkout.GBS.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
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
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Checkout.GBS.Filter
{
    public class PostFilters : ActionFilterAttribute, IFilterProvider
    {
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private ShoppingCartModelExtension _shoppingCartModel;
        

        public PostFilters(
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
            CustomerSettings customerSettings)
        {
            this._storeContext = storeContext;
            this._pluginFinder = pluginFinder;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._workContext = workContext;
            this._shoppingCartModel = new ShoppingCartModelExtension(
                    shoppingCartModelFactory,
                    pluginFinder,
                    productService,
                    storeContext,
                    workContext,
                    shoppingCartService,
                    pictureService,
                    localizationService,
                    productAttributeService,
                    productAttributeFormatter,
                    productAttributeParser,
                    taxService,
                    currencyService,
                    priceCalculationService,
                    priceFormatter,
                    checkoutAttributeParser,
                    checkoutAttributeFormatter,
                    orderProcessingService,
                    discountService,
                    customerService,
                    giftCardService,
                    dateRangeService,
                    countryService,
                    stateProvinceService,
                    shippingService,
                    orderTotalCalculationService,
                    checkoutAttributeService,
                    paymentService,
                    workflowMessageService,
                    permissionService,
                    downloadService,
                    cacheManager,
                    webHelper,
                    customerActivityService,
                    genericAttributeService,
                    addressAttributeFormatter,
                    httpContext,
                    mediaSettings,
                    shoppingCartSettings,
                    catalogSettings,
                    orderSettings,
                    shippingSettings,
                    taxSettings,
                    captchaSettings,
                    addressSettings,
                    rewardPointsSettings,
                    customerSettings);
        }

        public IEnumerable<System.Web.Mvc.Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
        
            if (controllerContext.Controller is Nop.Web.Controllers.CheckoutController &&
                actionDescriptor.ActionName.Equals("Confirm",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<System.Web.Mvc.Filter>() { new System.Web.Mvc.Filter(this, FilterScope.Action, 0) };
            }

            return new List<System.Web.Mvc.Filter>();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
           
              
              //cart[0].Product
                //switch (((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.Name)
                //{
                //    case "UpdateCart":
                //        FilterModel cartFilterModel = new FilterModel(_storeContext, _workContext);
                //        var cartResult = filterContext.Result as ViewResultBase;
                //        ShoppingCartModel cartModel = (ShoppingCartModel)cartResult.Model;
                //        cartModel.CustomProperties = cartFilterModel.GetNoteCardSetCount();
                //        break;
                //    case "UpdateWishlist":
                //        FilterModel wishlistFilterModel = new FilterModel(_storeContext, _workContext);
                //        var wishlistResult = filterContext.Result as ViewResultBase;
                //        WishlistModel wishlistModel = (WishlistModel)wishlistResult.Model;
                //        wishlistModel.CustomProperties = wishlistFilterModel.GetNoteCardSetCount();
                //        break;
                //    default:
                //        break;
                //}


            }
        }
    }
}
