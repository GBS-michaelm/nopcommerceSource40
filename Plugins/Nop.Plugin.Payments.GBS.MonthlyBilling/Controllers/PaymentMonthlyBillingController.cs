using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.GBS.MonthlyBilling.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.GBS.MonthlyBilling.Controllers
{
    public class GBSPaymentMonthlyBillingController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly HttpContextBase _httpContext;

        public GBSPaymentMonthlyBillingController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            HttpContextBase httpContext)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._httpContext = httpContext;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
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
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
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

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            
            //set postback values
            var form = this.Request.Form;
            model.MonthlyBillingName = form["MonthlyBillingName"];
            model.MonthlyBillingPhoneNumber = form["MonthlyBillingPhoneNumber"];
            model.MonthlyBillingReference = form["MonthlyBillingReference"];

            return View("~/Plugins/Payments.GBS.MonthlyBilling/Views/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo.CustomValues.Add("Monthly Billing Name", form["MonthlyBillingName"]);
            paymentInfo.CustomValues.Add("Monthly Billing Phone", form["MonthlyBillingPhoneNumber"]);
            paymentInfo.CustomValues.Add("Monthly Billing Reference", form["MonthlyBillingReference"]);
            _httpContext.Session["monthlyBillingName"] = form["MonthlyBillingName"];
            _httpContext.Session["monthlyBillingPhoneNumber"] = form["MonthlyBillingPhoneNumber"];
            _httpContext.Session["monthlyBillingReference"] = form["MonthlyBillingReference"];
            return paymentInfo;
        }
    }
}