using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.PriceCalculation.GBS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.PriceCalculation.GBS.Controllers
{
    [Area(AreaNames.Admin)]
    public class PriceCalculationConfigurationGBSController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;


        public PriceCalculationConfigurationGBSController(ILocalizationService localizationService,
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
            var GBSPriceCalculationSettings = _settingService.LoadSetting<GBSPriceCalculationSettings>(storeScope);

            var model = new ConfigurationModel
            {

                //GBSLoginWebServiceAddress = GBSPriceCalculationSettings.GBSLoginWebServiceAddress,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                
                //model.GBSLoginWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSPriceCalculationSettings, x => x.GBSLoginWebServiceAddress, storeScope);

            }

            return View("~/Plugins/PriceCalculation.GBS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSPriceCalculationSettings = _settingService.LoadSetting<GBSPriceCalculationSettings>(storeScope);

            //save settings

            //GBSPriceCalculationSettings.GBSLoginWebServiceAddress = model.GBSLoginWebServiceAddress;


            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */


            //if (model.GBSLoginWebServiceAddress_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(GBSPriceCalculationSettings, x => x.GBSLoginWebServiceAddress, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(GBSPriceCalculationSettings, x => x.GBSLoginWebServiceAddress, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

    }
        
}