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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Plugin.Widgets.CustomersCanvas.Controllers;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Newtonsoft.Json;
using Nop.Plugin.CanvasOverride.GBS.DataAccess;
using System.Data;

namespace Nop.Plugin.CanvasOverride.GBS
{
    public class PostFilters : ActionFilterAttribute, IFilterProvider
    {
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        

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

        }

        public IEnumerable<System.Web.Mvc.Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
        
            if (controllerContext.Controller is CcWidgetController &&
                actionDescriptor.ActionName.Equals("ProductDetailsAfterBreadcrumb",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<System.Web.Mvc.Filter>() { new System.Web.Mvc.Filter(this, FilterScope.Action, 0) };
            }

            return new List<System.Web.Mvc.Filter>();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var miscPlugins = _pluginFinder.GetPlugins<CanvasOverridePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {


                switch (((ReflectedActionDescriptor)filterContext.ActionDescriptor).MethodInfo.Name)
                {
                    case "ProductDetailsAfterBreadcrumb":
              
                        //var ccResult = filterContext.Result as ViewResultBase;
                        //CcEditorLoaderModel ccModel = (CcEditorLoaderModel)ccResult.Model;
                        //DBManager db = new DBManager();
                        //DataTable dt = new DataTable();
                        //Dictionary<string, Object> param = new Dictionary<string, Object>();
                        //Dictionary<string, string> parameterTypes = new Dictionary<string, string>();
                        //dt.Columns.Add("Sku", typeof(string));
                        //dt.Rows.Add("CMTHH1-001-100-01278");

                        //param.Add("@sku", dt);
                        //parameterTypes.Add("@sku", "dbo.ProductList");  

                        //param.Add("@id", "13073");

                        //var result = db.GetParameterizedReader("EXEC usp_SelectMarketCenterProducts @id, @sku", param, 2, parameterTypes);
                        //string editorJson = result["EditorJson"];
                    
                        //dynamic designData = JsonConvert.DeserializeObject<Object>(ccModel.Config);
                        //designData.config.userInfo = JsonConvert.DeserializeObject<Object>(editorJson);

                        //var serializedData = JsonConvert.SerializeObject(designData);



                        //ccModel.Config = serializedData;
                        break;
                    default:
                        break;
                }


            }
        }
    }
}
