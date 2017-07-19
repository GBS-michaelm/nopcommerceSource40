using System.Web.Mvc;
using Nop.Admin.Controllers;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Plugin.Shipping.GBS.Models;
using Nop.Plugin.Shipping.GBS.Models.Product;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Services.Stores;
using Nop.Core.Domain.Localization;
using System.Linq.Expressions;
using Antlr.Runtime.Misc;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Nop.Plugin.Shipping.GBS.Infrastructure;
using Nop.Services.Configuration;
using Nop.Plugin.DataAccess.GBS;

namespace Nop.Plugin.Shipping.GBS.Controllers
{
    public class ExtendedFieldsController : BaseAdminController
    {
        #region property

        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly GBSShippingSetting _gbsShippingSetting;
        public string _TableName = string.Empty;
        public ExtendedFieldsController(IProductService productService,
            ILocalizationService localizationService, 
            IStoreService storeService,
            IWorkContext workContext, 
            ISettingService settingService,
            GBSShippingSetting gbsShippingSetting)

        {
            this._productService = productService;
            this._localizationService = localizationService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._settingService = settingService;
            this._gbsShippingSetting = gbsShippingSetting;
        }

        #endregion

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    //little hack here
        //    //always set culture to 'en-US' (Telerik has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
        //    CommonHelper.SetTelerikCulture();

        //    base.Initialize(requestContext);
        //}

        public ActionResult ProductExtendedFields(int productId)
        {
            _TableName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Table.Name").ResourceValue.ToString();

            ProductGBSModel model = new ProductGBSModel();

            var product = _productService.GetProductById(productId);

            // Get the value from Database using Web API
            if (product != null)
            {
                // Get values from GBS Product TBL in nop
                model = DataManager.GetGBSShippingCategory(productId, _TableName);
                model.ShippingCategoryA = model.ShippingCategoryA == null ? "" : model.ShippingCategoryA.Trim();
                model.ShippingCategoryB = model.ShippingCategoryB == null ? "" : model.ShippingCategoryB.Trim();
                var _flag = model.ProductID == 0 ? "True" : "False";

                // Save value to the Nop Attribute for later use, so we don't have to make another call to DB
                var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
                genericAttributeService.SaveAttribute(product, "NewItemFlag", _flag);
                
                //Get value from NOP Arributes, which stored data in nop table   :: For furture reference
                //product.GetAttribute<string>("ShippingClass");
            }

            return View(model);
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSShippingSettings = _settingService.LoadSetting<GBSShippingSetting>(storeScope);

            var model = new ProductExtendedFieldsModel
            {
                LoginId = _gbsShippingSetting.LoginId,
                Password = _gbsShippingSetting.Password,
                GBSShippingWebServiceAddress = _gbsShippingSetting.GBSShippingWebServiceAddress,
                GBSStoreNamePrepend = _gbsShippingSetting.GBSStoreNamePrepend,
                ActiveStoreScopeConfiguration = storeScope,

                TableName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Table.Name").ResourceValue.ToString(),
                ShippingOptionDesError = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.ShippingOption.Description.Error").ResourceValue.ToString(),
                ShippingOptionDesName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.ShippingOption.Description.Name").ResourceValue.ToString(),
                ShippingOptionDesSuccess = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.ShippingOption.Description.Success").ResourceValue.ToString(),


                ShippingCategoryA = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.ShippingCategoryA").ResourceValue.ToString(),
                ShippingCategoryB = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.ShippingCategoryB").ResourceValue.ToString()

            };
            var product = _productService.GetProductById(1);
            if (product != null)
            {
                var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
                genericAttributeService.SaveAttribute(product, "TableName", model.TableName);
            }
            if (storeScope > 0)
            {

                model.LoginId_OverrideForStore = _settingService.SettingExists(GBSShippingSettings, x => x.LoginId, storeScope);
                model.Password_OverrideForStore = _settingService.SettingExists(GBSShippingSettings, x => x.Password, storeScope);
                model.GBSShippingWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSShippingSettings, x => x.GBSShippingWebServiceAddress, storeScope);
                model.GBSStoreNamePrepend_OverrideForStore = _settingService.SettingExists(GBSShippingSettings, x => x.GBSStoreNamePrepend, storeScope);
                model.UseFlatRate_OverrideForStore = _settingService.SettingExists(GBSShippingSettings, x => x.UseFlatRate, storeScope);
                model.FlatRateAmount_OverrideForStore = _settingService.SettingExists(GBSShippingSettings, x => x.FlatRateAmount, storeScope);
            }


            return View("~/Plugins/Shipping.GBS/Views/ExtendedFields/Configure.cshtml", model);

        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ProductExtendedFieldsModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSShippingSetting = _settingService.LoadSetting<GBSShippingSetting>(storeScope);

            // Get language ID
            var languageId = _workContext.WorkingLanguage.Id;

            // Save String Resources
            Builder("Plugins.Shipping.GBS.Product.ShippingOption.Description.Error", languageId, model.ShippingOptionDesError);
            Builder("Plugins.Shipping.GBS.Product.ShippingOption.Description.Success", languageId, model.ShippingOptionDesSuccess);
            Builder("Plugins.Shipping.GBS.Product.ShippingOption.Description.Name", languageId, model.ShippingOptionDesName);

            Builder("Plugins.Shipping.GBS.Product.Table.Name", languageId, model.TableName);
            Builder("Plugins.Shipping.GBS.Product.ShippingCategoryA", languageId, model.ShippingCategoryA);
            Builder("Plugins.Shipping.GBS.Product.ShippingCategoryB", languageId, model.ShippingCategoryB);

            //save settings

            GBSShippingSetting.LoginId = model.LoginId;
            GBSShippingSetting.Password = model.Password;
            GBSShippingSetting.GBSShippingWebServiceAddress= model.GBSShippingWebServiceAddress;
            GBSShippingSetting.GBSStoreNamePrepend = model.GBSStoreNamePrepend;


            ///* We do not clear cache after each setting update.
            // * This behavior can increase performance because cached settings will not be cleared 
            // * and loaded from database after each update */

            if (model.LoginId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSShippingSetting, x => x.LoginId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSShippingSetting, x => x.LoginId, storeScope);

            if (model.Password_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSShippingSetting, x => x.Password, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSShippingSetting, x => x.Password, storeScope);

            if (model.GBSShippingWebServiceAddress_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSShippingSetting, x => x.GBSShippingWebServiceAddress, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSShippingSetting, x => x.GBSShippingWebServiceAddress, storeScope);

            if (model.GBSStoreNamePrepend_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSShippingSetting, x => x.GBSStoreNamePrepend, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSShippingSetting, x => x.GBSStoreNamePrepend, storeScope);

            if (model.UseFlatRate_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSShippingSetting, x => x.UseFlatRate, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSShippingSetting, x => x.UseFlatRate, storeScope);

            if (model.FlatRateAmount_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSShippingSetting, x => x.FlatRateAmount, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSShippingSetting, x => x.FlatRateAmount, storeScope);

            //now clear settings cache
            _settingService.ClearCache();


            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public void Builder(string resourceName, int languageId, string UpdatedValue)
        {
            var resource = _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);

            if (resource != null && resource.ResourceValue != UpdatedValue)
            {
                //update
                resource.ResourceValue = UpdatedValue;
                _localizationService.UpdateLocaleStringResource(resource);

            }
        }

    }
}