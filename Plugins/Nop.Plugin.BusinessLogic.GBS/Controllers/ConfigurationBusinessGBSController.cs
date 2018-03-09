using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.BusinessLogic.GBS.Models;
//using Nop.Plugin.Payments.GBS.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
//using Nop.Services.Payments;
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
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Hack_OverrideForStore = _settingService.SettingExists(GBSBusinessLogicSettings, x => x.Hack, storeScope);
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
            
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
                         
            if (model.Hack_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSBusinessLogicSettings, x => x.Hack, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSBusinessLogicSettings, x => x.Hack, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
}
  