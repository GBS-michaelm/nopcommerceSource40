using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Order.GBS.Models;
//using Nop.Plugin.Payments.GBS.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
//using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Order.GBS.Controllers
{
    public class ConfigurationGBSController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;


        public ConfigurationGBSController(ILocalizationService localizationService,
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
            var GBSOrderSettings = _settingService.LoadSetting<GBSOrderSettings>(storeScope);

            var model = new ConfigurationModel
            {
                
                LoginId = GBSOrderSettings.LoginId,
                Password = GBSOrderSettings.Password,
                GBSOrderWebServiceAddress = GBSOrderSettings.GBSOrderWebServiceAddress,
                GBSPrintFileWebServiceAddress = GBSOrderSettings.GBSPrintFileWebServiceAddress,
                GBSStoreNamePrepend = GBSOrderSettings.GBSStoreNamePrepend,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                
                model.LoginId_OverrideForStore = _settingService.SettingExists(GBSOrderSettings, x => x.LoginId, storeScope);
                model.Password_OverrideForStore = _settingService.SettingExists(GBSOrderSettings, x => x.Password, storeScope);
                model.GBSOrderWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSOrderSettings, x => x.GBSOrderWebServiceAddress, storeScope);
                model.GBSPrintFileWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSOrderSettings, x => x.GBSPrintFileWebServiceAddress, storeScope);
                model.GBSStoreNamePrepend_OverrideForStore = _settingService.SettingExists(GBSOrderSettings, x => x.GBSStoreNamePrepend, storeScope);

            }

            return View("~/Plugins/Order.GBS/Views/OrderGBS/Configure.cshtml", model);
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
            var GBSOrderSettings = _settingService.LoadSetting<GBSOrderSettings>(storeScope);

            //save settings

            GBSOrderSettings.LoginId = model.LoginId;
            GBSOrderSettings.Password = model.Password;
            GBSOrderSettings.GBSOrderWebServiceAddress = model.GBSOrderWebServiceAddress;
            GBSOrderSettings.GBSPrintFileWebServiceAddress = model.GBSPrintFileWebServiceAddress;
            GBSOrderSettings.GBSStoreNamePrepend = model.GBSStoreNamePrepend;
            

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            
            if (model.LoginId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSOrderSettings, x => x.LoginId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSOrderSettings, x => x.LoginId, storeScope);

            if (model.Password_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSOrderSettings, x => x.Password, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSOrderSettings, x => x.Password, storeScope);

            if (model.GBSOrderWebServiceAddress_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSOrderSettings, x => x.GBSOrderWebServiceAddress, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSOrderSettings, x => x.GBSOrderWebServiceAddress, storeScope);

            if (model.GBSPrintFileWebServiceAddress_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSOrderSettings, x => x.GBSPrintFileWebServiceAddress, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSOrderSettings, x => x.GBSPrintFileWebServiceAddress, storeScope);

            if (model.GBSStoreNamePrepend_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSOrderSettings, x => x.GBSStoreNamePrepend, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSOrderSettings, x => x.GBSStoreNamePrepend, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
        
}