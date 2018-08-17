using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.GBS.Models;
using Nop.Plugin.Payments.GBS.Validators;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Payment.GBSPaymentGateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Nop.Plugin.Payments.GBS.Controllers
{
    public class PaymentGBSController : BasePaymentController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        private readonly GBSPaymentSettings _gbsPaymentSettings;

        public PaymentGBSController(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            GBSPaymentSettings gbsPaymentSettings,
            ILogger logger)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            this._gbsPaymentSettings = _settingService.LoadSetting<GBSPaymentSettings>(storeScope);
            this._logger = logger;
        }
        
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSPaymentSettings = _settingService.LoadSetting<GBSPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = GBSPaymentSettings.UseSandbox,
                UseSavedCards = GBSPaymentSettings.UseSavedCards,
                TransactModeId = Convert.ToInt32(GBSPaymentSettings.TransactMode),
                //TransactionKey = GBSPaymentSettings.TransactionKey,
                LoginId = GBSPaymentSettings.LoginId,
                Password = GBSPaymentSettings.Password,
                GBSPaymentWebServiceAddress = GBSPaymentSettings.GBSPaymentWebServiceAddress,
                //AdditionalFee = GBSPaymentSettings.AdditionalFee,
                //AdditionalFeePercentage = GBSPaymentSettings.AdditionalFeePercentage,
                TransactModeValues = GBSPaymentSettings.TransactMode.ToSelectList(),
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.UseSandbox, storeScope);
                model.UseSavedCards_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.UseSavedCards, storeScope);
                model.TransactModeId_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.TransactMode, storeScope);
                //model.TransactionKey_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.TransactionKey, storeScope);
                model.LoginId_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.LoginId, storeScope);
                model.Password_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.Password, storeScope);
                model.GBSPaymentWebServiceAddress_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.GBSPaymentWebServiceAddress, storeScope);
                //model.AdditionalFee_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.AdditionalFee, storeScope);
                //model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(GBSPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.GBS/Views/PaymentGBS/Configure.cshtml", model);
        }

        [HttpPost]      
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GBSPaymentSettings = _settingService.LoadSetting<GBSPaymentSettings>(storeScope);

            //save settings
            GBSPaymentSettings.UseSandbox = model.UseSandbox;
            GBSPaymentSettings.UseSavedCards = model.UseSavedCards;
            GBSPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
            //GBSPaymentSettings.TransactionKey = model.TransactionKey;
            GBSPaymentSettings.LoginId = model.LoginId;
            GBSPaymentSettings.Password = model.Password;
            GBSPaymentSettings.GBSPaymentWebServiceAddress = model.GBSPaymentWebServiceAddress;
            //GBSPaymentSettings.AdditionalFee = model.AdditionalFee;
            //GBSPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.UseSandbox_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSPaymentSettings, x => x.UseSandbox, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSPaymentSettings, x => x.UseSandbox, storeScope);

            if (model.UseSavedCards_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSPaymentSettings, x => x.UseSavedCards, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSPaymentSettings, x => x.UseSavedCards, storeScope);

            if (model.TransactModeId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSPaymentSettings, x => x.TransactMode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSPaymentSettings, x => x.TransactMode, storeScope);

            //if (model.TransactionKey_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(GBSPaymentSettings, x => x.TransactionKey, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(GBSPaymentSettings, x => x.TransactionKey, storeScope);

            if (model.LoginId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSPaymentSettings, x => x.LoginId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSPaymentSettings, x => x.LoginId, storeScope);

            if (model.Password_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSPaymentSettings, x => x.Password, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSPaymentSettings, x => x.Password, storeScope);

            if (model.GBSPaymentWebServiceAddress_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GBSPaymentSettings, x => x.GBSPaymentWebServiceAddress, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GBSPaymentSettings, x => x.GBSPaymentWebServiceAddress, storeScope);

            //if (model.AdditionalFee_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(GBSPaymentSettings, x => x.AdditionalFee, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(GBSPaymentSettings, x => x.AdditionalFee, storeScope);

            //if (model.AdditionalFeePercentage_OverrideForStore || storeScope == 0)
            //    _settingService.SaveSetting(GBSPaymentSettings, x => x.AdditionalFeePercentage, storeScope, false);
            //else if (storeScope > 0)
            //    _settingService.DeleteSetting(GBSPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
  
    }
}