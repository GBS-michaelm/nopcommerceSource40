using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.GBS.MonthlyBilling.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.GBS.MonthlyBilling.Controllers
{
    [Area(AreaNames.Admin)]
    public class GBSPaymentMonthlyBillingController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public GBSPaymentMonthlyBillingController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }
        
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var monthlyBillingPaymentSettings = _settingService.LoadSetting<GBSMonthlyBillingPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AdditionalFee = monthlyBillingPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = monthlyBillingPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = monthlyBillingPaymentSettings.ShippableProductRequired;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(monthlyBillingPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(monthlyBillingPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = _settingService.SettingExists(monthlyBillingPaymentSettings, x => x.ShippableProductRequired, storeScope);
            }

            return View("~/Plugins/Payments.GBS.MonthlyBilling/Views/Configure.cshtml", model);
        }

        [HttpPost]     
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var monthlyBillingPaymentSettings = _settingService.LoadSetting<GBSMonthlyBillingPaymentSettings>(storeScope);

            //save settings
            monthlyBillingPaymentSettings.AdditionalFee = model.AdditionalFee;
            monthlyBillingPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            monthlyBillingPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(monthlyBillingPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(monthlyBillingPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(monthlyBillingPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        } 
    }
}