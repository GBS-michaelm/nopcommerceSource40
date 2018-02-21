using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.GBS.Models;
using Nop.Plugin.Payments.GBS.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Core.Infrastructure;
using System.Data;
using Nop.Services.Logging;
using Payment.GBSPaymentGateway;

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
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
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
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
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

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            try
            {
              //  IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
                var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);

                int customerID = _workContext.CurrentCustomer.Id;
                ViewBag.UseSavedCards = _gbsPaymentSettings.UseSavedCards;
                ViewBag.StoreID = storeScope;

                if ((_gbsPaymentSettings.UseSavedCards)){

                    DBManager dbmanager = new DBManager();
                    Dictionary<string, string> paramDic = new Dictionary<string, string>();
                    paramDic.Add("@CustomerID", customerID.ToString());
                    string select = "exec usp_getCCProfiles @CustomerID";
                    DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);
                    ViewBag.SavedCCCount = dView.Count;



                }
            

                var model = new PaymentInfoModel();

                //years
                for (var i = 0; i < 15; i++)
                {
                    var year = Convert.ToString(DateTime.Now.Year + i);
                    model.ExpireYears.Add(new SelectListItem
                    {
                        Text = year,
                        Value = year,
                    });
                }

                //months
                for (var i = 1; i <= 12; i++)
                {
                    var text = (i < 10) ? "0" + i : i.ToString();
                    model.ExpireMonths.Add(new SelectListItem
                    {
                        Text = text,
                        Value = i.ToString(),
                    });
                }

                //set postback values
                var form = this.Request.Form;
                model.CardholderName = form["CardholderName"];
                model.CardNumber = form["CardNumber"];
                model.CardCode = form["CardCode"];
                var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));

                if (selectedMonth != null)
                    selectedMonth.Selected = true;

                var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));

                if (selectedYear != null)
                    selectedYear.Selected = true;
                return View("PaymentInfo", model);
            }catch(Exception ex)
            {
                _logger.Error("error in PaymentInfo()", ex, null);
                throw ex; 
           }
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            try
            {
                

                //validate
                var validator = new PaymentInfoValidator(_localizationService);


                var model = new PaymentInfoModel
                {
                    CardholderName = form["CardholderName"],
                    CardNumber = form["CardNumber"],
                    CardCode = form["CardCode"],
                    ExpireMonth = form["ExpireMonth"],
                    ExpireYear = form["ExpireYear"]
                };

                var validationResult = validator.Validate(model);
                var validationCheck = validationResult.IsValid;

                if (form.AllKeys.Contains("ProfileID") && !String.IsNullOrEmpty(form["ProfileID"].ToString()))
                {
                    validationCheck = true;
                }

                if (!validationCheck)
                    warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));
            }
            catch (Exception ex)
            {
                var innerMsg = "";
               if (ex.InnerException != null)
                {
                    innerMsg = ex.InnerException.Message;
                }

                _logger.Error("error in ValidatePaymentForm() --- " +ex.Message + " inner exception " +innerMsg, ex, null);
                throw ex;
            }

            return warnings;

        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {   
            var paymentInfo = new ProcessPaymentRequest();

            GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();

            if (form.AllKeys.Contains("ProfileID") && !String.IsNullOrEmpty(form["ProfileID"].ToString()))
            {
                //WebServices.Models.Payment.PaymentProfileModel paymentProfile = gateway.ReadProfile(int.Parse(form["ProfileID"]), _gbsPaymentSettings.GBSPaymentWebServiceAddress, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password, _gbsPaymentSettings.UseSandbox);

                paymentInfo = new ProcessPaymentRequest();

                //paymentInfo.CreditCardName = paymentProfile.customerName;
                //paymentInfo.CreditCardNumber = paymentProfile.cardNumber;
                //paymentInfo.CreditCardExpireMonth = int.Parse(paymentProfile.cardExpireMonth);
                //paymentInfo.CreditCardExpireYear = int.Parse(paymentProfile.cardExpireYear);
                paymentInfo.CustomValues["ProfileID"] = form["ProfileID"].ToString();
                paymentInfo.CustomValues["SavedProfile"] = true;
                paymentInfo.CustomValues["StoreProfile"] = form["StoreProfile"];
            }
            else
            {
                paymentInfo = new ProcessPaymentRequest();

                //paymentInfo.CreditCardType is not used by Authorize.NET
                paymentInfo.CreditCardName = form["CardholderName"];
                paymentInfo.CreditCardNumber = form["CardNumber"];
                paymentInfo.CreditCardExpireMonth = int.Parse(form["ExpireMonth"]);
                paymentInfo.CreditCardExpireYear = int.Parse(form["ExpireYear"]);
                paymentInfo.CreditCardCvv2 = form["CardCode"];
                paymentInfo.CustomValues["SavedProfile"] = false;
                paymentInfo.CustomValues["NickName"] = form["NickName"];
                paymentInfo.CustomValues["StoreProfile"] = form["StoreProfile"];
            }

            return paymentInfo;
        }
    }
}