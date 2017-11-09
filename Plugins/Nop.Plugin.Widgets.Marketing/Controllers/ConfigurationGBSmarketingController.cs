using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Marketing.Models;
//using Nop.Plugin.Payments.GBS.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
//using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Marketing.Controllers
{
    public class ConfigurationGBSMarketingController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;


        public ConfigurationGBSMarketingController(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSMarketingSettings = _settingService.LoadSetting<GBSMarketingSettings>(storeScope);

            var model = new ConfigurationModel
            {
                
                LoginId = GBSMarketingSettings.LoginId,
                Password = GBSMarketingSettings.Password,
                GBSMarketingWebServiceAddress = GBSMarketingSettings.GBSMarketingWebServiceAddress,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                
                model.LoginId_OverrideForStore = _settingService.SettingExists(GBSMarketingSettings, x => x.LoginId, storeScope);
                model.Password_OverrideForStore = _settingService.SettingExists(GBSMarketingSettings, x => x.Password, storeScope);
                model.GBSMarketingWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSMarketingSettings, x => x.GBSMarketingWebServiceAddress, storeScope);

            }

            return View("~/Plugins/Widgets.Marketing/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSMarketingSettings = _settingService.LoadSetting<GBSMarketingSettings>(storeScope);

            //save settings

            GBSMarketingSettings.LoginId = model.LoginId;
            GBSMarketingSettings.Password = model.Password;
            GBSMarketingSettings.GBSMarketingWebServiceAddress = model.GBSMarketingWebServiceAddress;
            

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            
            if (model.LoginId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSMarketingSettings, x => x.LoginId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSMarketingSettings, x => x.LoginId, storeScope);

            if (model.Password_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSMarketingSettings, x => x.Password, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSMarketingSettings, x => x.Password, storeScope);

            if (model.GBSMarketingWebServiceAddress_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSMarketingSettings, x => x.GBSMarketingWebServiceAddress, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSMarketingSettings, x => x.GBSMarketingWebServiceAddress, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
        
}