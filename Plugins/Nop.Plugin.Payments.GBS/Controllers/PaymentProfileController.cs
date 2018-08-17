using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.GBS.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Payment.GBSPaymentGateway;
using System;
using System.Collections.Generic;
using System.Data;
using WebServices.Models.Payment;

namespace Nop.Plugin.Payments.GBS.Controllers
{
    public class PaymentProfileController : BaseController
    {
        private readonly GBSPaymentSettings _gbsPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IEncryptionService _encryptionService;
        private readonly CurrencySettings _currencySettings;

        public PaymentProfileController(
            GBSPaymentSettings gbsPaymentSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            ILogger logger,
            ICustomerService customerService,
            IWebHelper webHelper,
            IOrderTotalCalculationService orderTotalCalculationService,
            IEncryptionService encryptionService,
            CurrencySettings currencySettings)
        {
            this._gbsPaymentSettings = gbsPaymentSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._logger = logger;
            this._customerService = customerService;
            this._webHelper = webHelper;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._encryptionService = encryptionService;
            this._currencySettings = currencySettings;
        }

        public IActionResult SaveCCView()
        {
            var model = new CustomerPaymentProfilesModel();

            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            model.customerID = _workContext.CurrentCustomer.Id;

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@CustomerID", model.customerID.ToString());
            string select = "SELECT * FROM Profiles WHERE CustomerID = " + model.customerID + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

            if (dView.Count > 0)
            {
                foreach (DataRow dRow in dView.Table.Rows)
                {
                    PaymentMethodModel getProfile = new PaymentMethodModel();
                    getProfile.profileID = (int)dRow["ProfileID"];
                    getProfile.NickName = dRow["NickName"].ToString();
                    getProfile.Last4Digits = dRow["Last4Digits"].ToString();
                    getProfile.CardType = dRow["CardType"].ToString();
                    getProfile.ExpMonth = (int)dRow["ExpMonth"];
                    getProfile.ExpYear = (int)dRow["ExpYear"];
                    model.SavedProfiles.Add(getProfile);
                }
            }


            return View("~/Plugins/Payments.GBS/Views/PaymentGBS/SaveCreditCard.cshtml", model);
        }

        #region My account / Payment methods

        public IActionResult PaymentMethods()
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerPaymentProfilesModel();

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@CustomerID", customer.Id.ToString());
            string select = "SELECT * FROM Profiles WHERE CustomerID = " + customer.Id + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

            if (dView != null && dView.Count > 0)
            {
                foreach (DataRow dRow in dView.Table.Rows)
                {
                    PaymentMethodModel getProfile = new PaymentMethodModel();
                    getProfile.profileID = (int)dRow["ProfileID"];
                    getProfile.NickName = dRow["NickName"].ToString();
                    getProfile.Last4Digits = dRow["Last4Digits"].ToString();
                    getProfile.CardType = dRow["CardType"].ToString();
                    getProfile.ExpMonth = (int)dRow["ExpMonth"];
                    getProfile.ExpYear = (int)dRow["ExpYear"];
                    model.SavedProfiles.Add(getProfile);
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult PaymentMethodDelete(int profileId)
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();
            var response = gateway.DeleteProfile(int.Parse(profileId.ToString()), _gbsPaymentSettings.GBSPaymentWebServiceAddress, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password, _gbsPaymentSettings.UseSandbox);

            if (response.procStatus == "0" || response.procStatus == "9581")
            {
                DBManager dbmanager = new DBManager();
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                paramDic.Add("@CustomerID", customer.Id.ToString());
                paramDic.Add("@ProfileID", profileId.ToString());
                string select = "DELETE FROM Profiles WHERE CustomerID = " + customer.Id + " AND ProfileID = " + profileId + "";
                DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);
            }
            else
            {
                //ADD ERROR CODE HERE
            }

            //redirect to the payment methods list page
            return Json(new
            {
                redirect = Url.RouteUrl("CustomerPaymentMethods"),
            });
        }

        public IActionResult PaymentMethodAdd()
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var model = new CustomerPaymentProfilesModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult PaymentMethodAdd(PaymentProfileModel model, IFormCollection form)
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var result = new List<string>();

            //form.("CardholderName", "Default");
            //form.Add("ProfileID", "");

            GBSPaymentGateway paymentController = new GBSPaymentGateway(_settingService, _currencyService, _customerService, _webHelper, _orderTotalCalculationService, _encryptionService, _currencySettings, _gbsPaymentSettings, _localizationService, _logger);
            result = (List<string>)paymentController.ValidatePaymentForm(form);
            ViewBag.warnings = result;


            PaymentProfileModel profile = new PaymentProfileModel();
            profile.cardNumber = form["CardNumber"].ToString();
            profile.cardExpireMonth = form["ExpireMonth"].ToString().Length == 1 ? "0" + form["ExpireMonth"].ToString() : form["ExpireMonth"].ToString(); //prepend 0 for single digit months
            profile.cardExpireYear = form["ExpireYear"].ToString();
            profile.profileAction = "C";
            profile.accountType = "CC";
            profile.status = "A";
            profile.autoGenerate = true;
            profile.sandBox = _gbsPaymentSettings.UseSandbox;

            GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();
            var response = gateway.CreateProfile(profile, _gbsPaymentSettings.GBSPaymentWebServiceAddress, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password);

            if (response.procStatus == "0")
            {
                string cardType;
                string firstDigit = response.cardNumber.ToString();
                firstDigit = firstDigit.Substring(0, firstDigit.Length - (firstDigit.Length - 1));
                switch (firstDigit)
                {
                    case "4":
                        cardType = "VI";
                        break;
                    case "5":
                        cardType = "MC";
                        break;
                    case "3":
                        cardType = "AX";
                        break;
                    case "6":
                        cardType = "DI";
                        break;
                    default:
                        cardType = "VI";
                        break;
                }

                string profileID = response.profileID.ToString();
                string nickName = form["NickName"].ToString();
                nickName = nickName.Replace("'", "''");
                string last4Digits = response.cardNumber.ToString();
                last4Digits = last4Digits.Substring(last4Digits.Length - 4);
                string expireMonth = profile.cardExpireMonth;
                string expireYear = profile.cardExpireYear;

                DBManager manager = new DBManager();
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                paramDic.Add("@CustomerID", customer.Id.ToString());
                paramDic.Add("@ProfileID", profileID.ToString());
                paramDic.Add("@NickName", nickName.ToString());
                paramDic.Add("@Last4Digits", last4Digits.ToString());
                paramDic.Add("@CardType", cardType.ToString());
                paramDic.Add("@ExpMonth", expireMonth.ToString());
                paramDic.Add("@ExpYear", expireYear.ToString());

                string insert = "INSERT INTO Profiles (CustomerID, ProfileID, NickName, Last4Digits, CardType, ExpMonth, ExpYear) ";
                insert += "VALUES ('" + customer.Id + "', '" + profileID.ToString() + "', '" + nickName.ToString() + "', '" + last4Digits.ToString() + "', '" + cardType.ToString() + "', '" + expireMonth.ToString() + "', '" + expireYear.ToString() + "')";
                manager.SetParameterizedQueryNoData(insert, paramDic);
            }
            else
            {
                //ADD ERROR CODE HERE
                ViewBag.warnings.Add(response.profileMessage);
            }

            if (ViewBag.warnings.Count > 0)
            {
                return View("PaymentMethodAdd");
            }
            else
            {
                return RedirectToRoute("CustomerPaymentMethods");
            }

        }

        public IActionResult PaymentMethodEdit(int profileId)
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerPaymentProfilesModel();

            GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();
            var response = gateway.ReadProfile(int.Parse(profileId.ToString()), _gbsPaymentSettings.GBSPaymentWebServiceAddress, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password, _gbsPaymentSettings.UseSandbox);

            if (response.procStatus == "0")
            {
                DBManager dbmanager = new DBManager();
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                paramDic.Add("@CustomerID", customer.Id.ToString());
                paramDic.Add("@ProfileID", profileId.ToString());
                string select = "SELECT * FROM Profiles WHERE CustomerID = " + customer.Id + " AND ProfileID = " + profileId + "";
                DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);
                DataRow firstRow = dView.Table.Rows[0];

                PaymentMethodModel getProfile = new PaymentMethodModel();
                getProfile.profileID = Convert.ToInt32(response.profileID);
                getProfile.NickName = firstRow["NickName"].ToString();
                getProfile.Last4Digits = response.cardNumber.ToString();
                getProfile.CardType = firstRow["CardType"].ToString();
                getProfile.ExpMonth = (int)firstRow["ExpMonth"];
                getProfile.ExpYear = (int)firstRow["ExpYear"];
                model.SavedProfiles.Add(getProfile);
            }
            else
            {
                //ADD ERROR CODE HERE
                ViewBag.warnings = response.profileMessage;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult PaymentMethodEdit(CustomerPaymentProfilesModel model, int profileId, IFormCollection form)
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var result = new List<string>();

            //form.Add("CardholderName", "Default");
            //form.Add("ProfileID", "");

            GBSPaymentGateway paymentController = new GBSPaymentGateway(_settingService, _currencyService, _customerService, _webHelper, _orderTotalCalculationService, _encryptionService, _currencySettings, _gbsPaymentSettings, _localizationService, _logger);
            result = (List<string>)paymentController.ValidatePaymentForm(form);
            ViewBag.warnings = result;

            PaymentProfileModel profile = new PaymentProfileModel();
            profile.profileID = profileId.ToString();
            profile.cardNumber = form["CardNumber"].ToString();
            profile.cardExpireMonth = form["ExpireMonth"].ToString().Length == 1 ? "0" + form["ExpireMonth"].ToString() : form["ExpireMonth"].ToString(); //prepend 0 for single digit months
            profile.cardExpireYear = form["ExpireYear"].ToString();
            profile.profileAction = "U";
            profile.accountType = "CC";
            profile.status = "A";
            profile.autoGenerate = true;

            GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();
            var response = gateway.UpdateProfile(profile, _gbsPaymentSettings.GBSPaymentWebServiceAddress, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password, _gbsPaymentSettings.UseSandbox);

            if (response.procStatus == "0")
            {
                string cardType;
                string firstDigit = response.cardNumber.ToString();
                firstDigit = firstDigit.Substring(0, firstDigit.Length - (firstDigit.Length - 1));
                switch (firstDigit)
                {
                    case "4":
                        cardType = "VI";
                        break;
                    case "5":
                        cardType = "MC";
                        break;
                    case "3":
                        cardType = "AX";
                        break;
                    case "6":
                        cardType = "DI";
                        break;
                    default:
                        cardType = "VI";
                        break;
                }

                string profileID = response.profileID.ToString();
                string nickName = form["NickName"].ToString();
                string last4Digits = response.cardNumber.ToString();
                last4Digits = last4Digits.Substring(last4Digits.Length - 4);
                string expireMonth = profile.cardExpireMonth;
                string expireYear = profile.cardExpireYear;

                DBManager manager = new DBManager();
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                paramDic.Add("@CustomerID", customer.Id.ToString());
                paramDic.Add("@ProfileID", profileID.ToString());
                paramDic.Add("@NickName", nickName.ToString());
                paramDic.Add("@Last4Digits", last4Digits.ToString());
                paramDic.Add("@CardType", cardType.ToString());
                paramDic.Add("@ExpMonth", expireMonth.ToString());
                paramDic.Add("@ExpYear", expireYear.ToString());

                string update = "UPDATE Profiles SET NickName='" + nickName.ToString() + "', Last4Digits='" + last4Digits.ToString() + "', CardType='" + cardType.ToString() + "', ExpMonth='" + expireMonth.ToString() + "', ExpYear='" + expireYear.ToString() + "'";
                update += "WHERE CustomerID = " + customer.Id + " AND ProfileID = " + profileID + "";
                manager.SetParameterizedQueryNoData(update, paramDic);
            }
            else
            {
                //ADD ERROR CODE HERE
                ViewBag.warnings.Add(response.profileMessage);
            }

            if (ViewBag.warnings.Count > 0)
            {
                return PaymentMethodEdit(profileId);
            }
            else
            {
                return RedirectToRoute("CustomerPaymentMethods");
            }
        }

        #endregion

    }
}
