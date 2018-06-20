using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.GBS.PurchaseOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.GBS.PurchaseOrder.Controllers
{
    [Area(AreaNames.Admin)]
    public class GBSPaymentPurchaseOrderController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public GBSPaymentPurchaseOrderController(IWorkContext workContext,
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
            var purchaseOrderPaymentSettings = _settingService.LoadSetting<GBSPurchaseOrderPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AdditionalFee = purchaseOrderPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = purchaseOrderPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = purchaseOrderPaymentSettings.ShippableProductRequired;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(purchaseOrderPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(purchaseOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = _settingService.SettingExists(purchaseOrderPaymentSettings, x => x.ShippableProductRequired, storeScope);
            }

            return View("~/Plugins/Payments.GBS.PurchaseOrder/Configure.cshtml", model);
        }

        [HttpPost]       
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var purchaseOrderPaymentSettings = _settingService.LoadSetting<GBSPurchaseOrderPaymentSettings>(storeScope);

            //save settings
            purchaseOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            purchaseOrderPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            purchaseOrderPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(purchaseOrderPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(purchaseOrderPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore , storeScope, false);
            _settingService.SaveSettingOverridablePerStore(purchaseOrderPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

      
    }
}