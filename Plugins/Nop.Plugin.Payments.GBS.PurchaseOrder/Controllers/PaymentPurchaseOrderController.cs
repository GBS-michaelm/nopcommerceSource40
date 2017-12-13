using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.GBS.PurchaseOrder.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.GBS.PurchaseOrder.Controllers
{
    public class GBSPaymentPurchaseOrderController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly HttpContextBase _httpContext;

        public GBSPaymentPurchaseOrderController(IWorkContext workContext,
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

            return View("~/Plugins/Payments.GBS.PurchaseOrder/Views/Configure.cshtml", model);
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

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            
            //set postback values
            var form = this.Request.Form;
            model.PurchaseOrderNumber = form["PurchaseOrderNumber"];
            model.PurchaseOrderName = form["PurchaseOrderName"];
            model.PurchaseOrderPhoneNumber = form["PurchaseOrderPhoneNumber"];

            if (_httpContext.Session["purchaseOrderNumber"] != null)
            {
                model.PurchaseOrderNumber = _httpContext.Session["purchaseOrderNumber"].ToString();
            }
            if (_httpContext.Session["purchaseOrderName"] != null)
            {
                model.PurchaseOrderName = _httpContext.Session["purchaseOrderName"].ToString();
            }
            if (_httpContext.Session["purchaseOrderPhoneNumber"] != null)
            {
                model.PurchaseOrderPhoneNumber = _httpContext.Session["purchaseOrderPhoneNumber"].ToString();
            }

            return View("~/Plugins/Payments.GBS.PurchaseOrder/Views/PaymentInfo.cshtml", model);
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
            paymentInfo.CustomValues.Add("PO Number", form["PurchaseOrderNumber"]);
            paymentInfo.CustomValues.Add("PO Name", form["PurchaseOrderName"]);
            paymentInfo.CustomValues.Add("PO Phone", form["PurchaseOrderPhoneNumber"]);
            _httpContext.Session["purchaseOrderNumber"] = form["PurchaseOrderNumber"];
            _httpContext.Session["purchaseOrderName"] = form["PurchaseOrderName"];
            _httpContext.Session["purchaseOrderPhoneNumber"] = form["PurchaseOrderPhoneNumber"];
            return paymentInfo;
        }
    }
}