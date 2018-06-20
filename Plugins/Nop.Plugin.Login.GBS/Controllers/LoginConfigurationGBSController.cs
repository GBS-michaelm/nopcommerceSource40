using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Login.GBS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;


namespace Nop.Plugin.Login.GBS.Controllers
{
    [Area(AreaNames.Admin)]
    public class LoginConfigurationGBSController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;


        public LoginConfigurationGBSController(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSLoginSettings = _settingService.LoadSetting<GBSLoginSettings>(storeScope);

            var model = new ConfigurationModel
            {

                GBSLoginWebServiceAddress = GBSLoginSettings.GBSLoginWebServiceAddress,
                GBSUpdateCustomerWebService = GBSLoginSettings.GBSUpdateCustomerWebService,
                GBSRegisterWebService = GBSLoginSettings.GBSRegisterWebService,
               GBSCustomerWebServiceUserName = GBSLoginSettings.GBSCustomerWebServiceUserName,
               GBSCustomerWebServicePassword = GBSLoginSettings.GBSCustomerWebServicePassword,
                //GBSOldPassword = GBSLoginSettings.OldPassword,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                
                model.GBSLoginWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSLoginSettings, x => x.GBSLoginWebServiceAddress, storeScope);
                model.GBSUpdateCustomerWebService_OverrideForStore = _settingService.SettingExists(GBSLoginSettings, x => x.GBSUpdateCustomerWebService, storeScope);
                model.GBSRegisterWebService_OverrideForStore = _settingService.SettingExists(GBSLoginSettings, x => x.GBSRegisterWebService, storeScope);
                model.GBSCustomerWebServiceName_OverrideForStore = _settingService.SettingExists(GBSLoginSettings, x => x.GBSCustomerWebServiceUserName, storeScope);
                model.GBSCustomerWebServicePassword_OverrideForStore = _settingService.SettingExists(GBSLoginSettings, x => x.GBSCustomerWebServicePassword, storeScope);
                //model.GBSOldPassword_OverrideForStore = _settingService.SettingExists(GBSLoginSettings, x => x.OldPassword, storeScope);

            }

            return View("~/Plugins/Login.GBS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSLoginSettings = _settingService.LoadSetting<GBSLoginSettings>(storeScope);

            //save settings

            GBSLoginSettings.GBSLoginWebServiceAddress = model.GBSLoginWebServiceAddress;
            GBSLoginSettings.GBSUpdateCustomerWebService = model.GBSUpdateCustomerWebService;
            GBSLoginSettings.GBSRegisterWebService = model.GBSRegisterWebService;
            GBSLoginSettings.GBSCustomerWebServiceUserName = model.GBSCustomerWebServiceUserName;
            GBSLoginSettings.GBSCustomerWebServicePassword = model.GBSCustomerWebServicePassword;
            //GBSLoginSettings.OldPassword = model.GBSOldPassword;


            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */


            if (model.GBSLoginWebServiceAddress_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSLoginSettings, x => x.GBSLoginWebServiceAddress, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSLoginSettings, x => x.GBSLoginWebServiceAddress, storeScope);

            if (model.GBSUpdateCustomerWebService_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSLoginSettings, x => x.GBSUpdateCustomerWebService, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSLoginSettings, x => x.GBSUpdateCustomerWebService, storeScope);

            if (model.GBSRegisterWebService_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSLoginSettings, x => x.GBSRegisterWebService, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSLoginSettings, x => x.GBSRegisterWebService, storeScope);

            if (model.GBSCustomerWebServiceName_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSLoginSettings, x => x.GBSCustomerWebServiceUserName, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSLoginSettings, x => x.GBSCustomerWebServiceUserName, storeScope);

            if (model.GBSCustomerWebServicePassword_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSLoginSettings, x => x.GBSCustomerWebServicePassword, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSLoginSettings, x => x.GBSCustomerWebServicePassword, storeScope);

            //if (model.GBSOldPassword_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(GBSLoginSettings, x => x.OldPassword, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(GBSLoginSettings, x => x.OldPassword, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
        
}