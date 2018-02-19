﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.GBS.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using WebServices.Models.Payment;
using Payment.GBSPaymentGateway;

namespace Nop.Plugin.Payments.GBS.Controllers
{
    public class PaymentProfileController : BaseController
    {
        private readonly GBSPaymentSettings _gbsPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public PaymentProfileController(
            GBSPaymentSettings gbsPaymentSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            ILogger logger)
        {
            this._gbsPaymentSettings = gbsPaymentSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._logger = logger;
        }

        public ActionResult SaveCCView()
        {
            var model = new CustomerPaymentProfilesModel();

            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            model.customerID = _workContext.CurrentCustomer.Id;

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@CustomerID", model.customerID.ToString());
            string select = "exec usp_getCCProfiles = " + model.customerID + "";
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

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PaymentMethods()
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerPaymentProfilesModel();

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@CustomerID", customer.Id.ToString());
            string select = "exec usp_getCCProfiles = " + customer.Id + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

            if (dView != null &&  dView.Count > 0)
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
        [PublicAntiForgery]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PaymentMethodDelete(int profileId)
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
                string select = "usp_deleteCCProfile " + customer.Id + "," + profileId + "";
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

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PaymentMethodAdd()
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var model = new CustomerPaymentProfilesModel();

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public ActionResult PaymentMethodAdd(PaymentProfileModel model, FormCollection form)
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var result = new List<string>();

            form.Add("CardholderName", "Default");
            form.Add("ProfileID", "");

            PaymentGBSController paymentController = new PaymentGBSController(_localizationService,_settingService,_storeService,_workContext,_gbsPaymentSettings,_logger);
            result = (List<string>) paymentController.ValidatePaymentForm(form);
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
                firstDigit = firstDigit.Substring(0,firstDigit.Length - (firstDigit.Length - 1));
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
                string billingAddressID = 

                DBManager manager = new DBManager();
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                paramDic.Add("@CustomerID", customer.Id.ToString());
                paramDic.Add("@ProfileID", profileID.ToString());
                paramDic.Add("@NickName", nickName.ToString());
                paramDic.Add("@Last4Digits", last4Digits.ToString());
                paramDic.Add("@CardType", cardType.ToString());
                paramDic.Add("@ExpMonth", expireMonth.ToString());
                paramDic.Add("@ExpYear", expireYear.ToString());

                //string insert = "INSERT INTO Profiles (CustomerID, ProfileID, NickName, Last4Digits, CardType, ExpMonth, ExpYear) ";
                //insert += "VALUES ('" + customer.Id + "', '" + profileID.ToString() + "', '" + nickName.ToString() + "', '" + last4Digits.ToString() + "', '" + cardType.ToString() + "', '" + expireMonth.ToString() + "', '" + expireYear.ToString() + "')";
                string insert = "usp_InsertCCProfile " + customer.Id + "', '" + profileID.ToString() + "', '" + nickName.ToString() + "', '" + last4Digits.ToString() + "', '" + cardType.ToString() + "', '" + expireMonth.ToString() + "', '" + expireYear.ToString() + "'";
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

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PaymentMethodEdit(int profileId)
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
                string select = "exec usp_getCCProfiles " + customer.Id + "," + profileId + "";
                DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);
                DataRow firstRow = dView.Table.Rows[0];

                PaymentMethodModel getProfile = new PaymentMethodModel();
                getProfile.profileID = Convert.ToInt32(response.profileID);
                getProfile.NickName = firstRow["NickName"].ToString();
                getProfile.Last4Digits = firstRow["Last4Digits"].ToString();
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
        [PublicAntiForgery]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PaymentMethodEdit(CustomerPaymentProfilesModel model, int profileId, FormCollection form)
        {
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            var customer = _workContext.CurrentCustomer;

            var result = new List<string>();

            form.Add("CardholderName", "Default");
            form.Add("ProfileID", "");

            PaymentGBSController paymentController = new PaymentGBSController(_localizationService, _settingService, _storeService, _workContext, _gbsPaymentSettings, _logger);
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
                firstDigit = firstDigit.Substring(0,firstDigit.Length - (firstDigit.Length - 1));
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
