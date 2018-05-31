using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.BusinessLogic.GBS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.BusinessLogic.GBS.Controllers
{
    public class ConfigurationBusinessGBSController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;


        public ConfigurationBusinessGBSController(ILocalizationService localizationService,
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
            var GBSBusinessLogicSettings = _settingService.LoadSetting<GBSBusinessLogicSettings>(storeScope);

            var model = new ConfigurationModel
            {

                Hack = GBSBusinessLogicSettings.Hack,
                MarketCenterDefaultId = GBSBusinessLogicSettings.MarketCenterDefaultId,
                MarketCenterNameBadgeDefaultId = GBSBusinessLogicSettings.MarketCenterNameBadgeDefaultId,
                MarketCenterBusinessCardDefaultId = GBSBusinessLogicSettings.MarketCenterBusinessCardDefaultId,
                MarketCenterCarMagnetDefaultId = GBSBusinessLogicSettings.MarketCenterCarMagnetDefaultId,

                MarketCenterWhatAmIReferenceName = GBSBusinessLogicSettings.MarketCenterWhatAmIReferenceName,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Hack_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.Hack, storeScope);
                model.MarketCenterDefaultId_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.MarketCenterDefaultId, storeScope);
                model.MarketCenterNameBadgeDefaultId_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.MarketCenterNameBadgeDefaultId, storeScope);               
                model.MarketCenterBusinessCardDefaultId_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.MarketCenterBusinessCardDefaultId, storeScope);
                model.MarketCenterCarMagnetDefaultId_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.MarketCenterCarMagnetDefaultId, storeScope);

                model.MarketCenterWhatAmIReferenceName_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.MarketCenterWhatAmIReferenceName, storeScope);
            }

            return View("~/Plugins/BusinessLogic.GBS/Views/BusinessLogic/Configure.cshtml", model);
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
            var GBSBusinessLogicSettings = _settingService.LoadSetting<GBSBusinessLogicSettings>(storeScope);

            //save settings

            GBSBusinessLogicSettings.Hack = model.Hack;
            GBSBusinessLogicSettings.MarketCenterDefaultId = model.MarketCenterDefaultId;
            GBSBusinessLogicSettings.MarketCenterNameBadgeDefaultId = model.MarketCenterNameBadgeDefaultId;                   
            GBSBusinessLogicSettings.MarketCenterBusinessCardDefaultId = model.MarketCenterBusinessCardDefaultId;            
            GBSBusinessLogicSettings.MarketCenterCarMagnetDefaultId = model.MarketCenterCarMagnetDefaultId;

            GBSBusinessLogicSettings.MarketCenterWhatAmIReferenceName = model.MarketCenterWhatAmIReferenceName;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            if (model.Hack_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.Hack, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.Hack, storeScope);

            if (model.MarketCenterDefaultId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.MarketCenterDefaultId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.MarketCenterDefaultId, storeScope);
                        
            if (model.MarketCenterNameBadgeDefaultId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.MarketCenterNameBadgeDefaultId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.MarketCenterNameBadgeDefaultId, storeScope);
            
            if (model.MarketCenterBusinessCardDefaultId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.MarketCenterBusinessCardDefaultId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.MarketCenterBusinessCardDefaultId, storeScope);            
            if (model.MarketCenterCarMagnetDefaultId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.MarketCenterCarMagnetDefaultId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.MarketCenterCarMagnetDefaultId, storeScope);

            if (model.MarketCenterWhatAmIReferenceName_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.MarketCenterWhatAmIReferenceName, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.MarketCenterWhatAmIReferenceName, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
}
  