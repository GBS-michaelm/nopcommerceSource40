using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Nop.Plugin.Widgets.Marketing.EmailPref.Models;
using Nop.Web.Framework.Controllers;
using GBSMarketingService;
using MarketingService;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Logging;
using System.Web;
using WebServices.Models;
using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Marketing.Controllers
{
    public class EmailPrefController : BasePluginController
    {

        private readonly GBSMarketingSettings _gbsMarketingSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public EmailPrefController(
            GBSMarketingSettings gbsMarketingSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            ILogger logger)
        {
            this._gbsMarketingSettings = gbsMarketingSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._logger = logger;
        }

        public ActionResult EmailPreferencesView()
        {
            string Email = System.Web.HttpContext.Current.Request.QueryString["email"];
            //string Website = System.Web.HttpContext.Current.Request.QueryString["website"];
            string Website = "HOM";

            if (Email == "" || Website == "" || Email == null || Website == null)
            {
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = "missing email address and/or website";

                return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
            }

            GBSMarketingServiceClient marketingClient = new GBSMarketingServiceClient();
            MarketingContactModel marketingContact = marketingClient.getMarketingContact(_gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password, Email, Website);
            if (marketingContact.error)
            {
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = marketingContact.errorMessage;

                return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
            }

            var model = new EmailPreferencesModel();
            model.ContactPref = marketingContact;

            model.subscribeStatusMaster = model.ContactPref.subscribeStatus;

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            string select = "EXEC usp_getEmailLists 'HOM'";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);

            if (dView.Count > 0)
            {
                foreach (DataRow dRow in dView.Table.Rows)
                {
                    EmailListModel getList = new EmailListModel();
                    getList.Id = (int)dRow["Id"];
                    getList.ListHeader = dRow["ListHeader"].ToString();
                    getList.ListTag = dRow["ListTag"].ToString();
                    getList.ListDetails = dRow["ListDetails"].ToString();
                    getList.ListImage = dRow["ListImage"].ToString();
                    getList.SubListName = dRow["SubListName"].ToString();
                    getList.UnSubListName = dRow["UnSubListName"].ToString();
                    getList.SendWeekly = (bool)dRow["SendWeekly"];
                    getList.SendMonthly = (bool)dRow["SendMonthly"];
                    getList.Unsubscribe = (bool)dRow["Unsubscribe"];
                    getList.Master = (bool)dRow["Master"];
                    getList.ListWebsite = dRow["ListWebsite"].ToString();
                    getList.Active = (bool)dRow["Active"];

                    foreach (MarketingListModel item in model.ContactPref.marketingLists)
                    {
                        if (item.name == getList.SubListName)
                        {
                            getList.listSubscribeStatus = true;
                        }

                        if (item.name == getList.UnSubListName)
                        {
                            getList.listSubscribeStatus = false;
                        }
                    }

                    model.ShowLists.Add(getList);
                }
            }

            MarketingContactModel marketingContact2 = marketingClient.getMarketingContact(_gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password, System.Web.HttpContext.Current.Request.QueryString["email"], "NCC");
            if (marketingContact2.error)
            {
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = marketingContact2.errorMessage;

                return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
            }

            model.ContactPref = marketingContact2;

            model.subscribeStatusPartner = model.ContactPref.subscribeStatus;

            DBManager dbmanager2 = new DBManager();
            Dictionary<string, string> paramDic2 = new Dictionary<string, string>();
            string select2 = "EXEC usp_getEmailLists 'NCC'";
            DataView dView2 = dbmanager2.GetParameterizedDataView(select2, paramDic2);

            if (dView2.Count > 0)
            {
                foreach (DataRow dRow in dView2.Table.Rows)
                {
                    EmailListModel getList2 = new EmailListModel();
                    getList2.Id = (int)dRow["Id"];
                    getList2.ListHeader = dRow["ListHeader"].ToString();
                    getList2.ListTag = dRow["ListTag"].ToString();
                    getList2.ListDetails = dRow["ListDetails"].ToString();
                    getList2.ListImage = dRow["ListImage"].ToString();
                    getList2.SubListName = dRow["SubListName"].ToString();
                    getList2.UnSubListName = dRow["UnSubListName"].ToString();
                    getList2.SendWeekly = (bool)dRow["SendWeekly"];
                    getList2.SendMonthly = (bool)dRow["SendMonthly"];
                    getList2.Unsubscribe = (bool)dRow["Unsubscribe"];
                    getList2.Master = (bool)dRow["Master"];
                    getList2.ListWebsite = dRow["ListWebsite"].ToString();
                    getList2.Active = (bool)dRow["Active"];

                    foreach (MarketingListModel item in model.ContactPref.marketingLists)
                    {
                        if (item.name == getList2.SubListName)
                        {
                            getList2.listSubscribeStatus = true;
                        }
                        if (item.name == getList2.UnSubListName)
                        {
                            getList2.listSubscribeStatus = false;
                        }
                    }

                    model.ShowPartnerLists.Add(getList2);
                }
            }

            return View("~/Plugins/Widgets.Marketing/Views/PublicInfo.cshtml", model);
        }


        public ActionResult EmailPreferencesEdit(FormCollection form)
        {
            string EmailAddress = form["EmailAddress"].ToString();
            string WebsiteMaster = form["WebsiteMaster"].ToString();
            string WebsitePartner = form["WebsitePartner"].ToString();
            bool IsSubscribedMaster = true;
            if (form["SubscribedMaster"].ToString() == "unsubscribed")
            {
                IsSubscribedMaster = false;
            }
            bool IsSubscribedPartner = true;
            if (form["SubscribedPartner"].ToString() == "unsubscribed")
            {
                IsSubscribedPartner = false;
            }

            //--Setup Master-------------//
            string MasterID = form["MasterID"].ToString();
            string SelectedListMaster = form[MasterID].ToString();
            bool unsubscribeMaster = false;
            bool sendMonthlyMaster = false;
            if (SelectedListMaster == "sendmonthly")
            {
                sendMonthlyMaster = true;
            }
            else if (SelectedListMaster == "unsubscribe")
            {
                unsubscribeMaster = true;
            }

            //--Setup Master Child-------------//
            string ChildID = form["ChildID"].ToString();
            string SelectedListChild = form[ChildID].ToString();
            bool unsubscribeChild = false;
            bool sendWeeklyChild = false;
            if (SelectedListChild == "sendweekly")
            {
                sendWeeklyChild = true;
            }
            else if (SelectedListChild == "unsubscribe")
            {
                unsubscribeChild = true;
            }

            //--Setup Partner-------------//
            string MasterPartnerID = form["MasterPartnerID"].ToString();
            string SelectedListPartner = form[MasterPartnerID].ToString();
            bool unsubscribePartner = false;
            bool sendMonthlyPartner = false;
            if (SelectedListPartner == "sendmonthly")
            {
                sendMonthlyPartner = true;
            }
            else if (SelectedListPartner == "unsubscribe")
            {
                unsubscribePartner = true;
            }

            //--Setup Partner Child-------------//



            GBSMarketingServiceClient marketingClient = new GBSMarketingServiceClient();
            MarketingContactModel marketingModel = new MarketingContactModel();
            MarketingListModel marketingListModel = new MarketingListModel();
            dynamic json = null;
            marketingModel.emailAddress = EmailAddress.ToString();
            marketingModel.providerId = EmailAddress.ToString();


            //--Master Call------------------------//
            marketingModel.website = WebsiteMaster.ToString();
            if (unsubscribeMaster == true)
            {
                marketingModel.subscribeStatus = "unsubscribed";
                marketingModel.invalid = "true";
                marketingModel.forceSubscribe = "false";
            }
            else
            {
                marketingModel.subscribeStatus = "subscribed";
                marketingModel.invalid = "false";
                marketingModel.forceSubscribe = "true";
            }
            if (sendMonthlyMaster == true)
            {
                marketingModel.listName = "Exclude_optdowns";
                marketingModel.listNameStatus = "true";

                marketingListModel = new MarketingListModel();
                marketingListModel.name = "Exclude_optdowns";
                marketingListModel.subscribeStatus = true;
                marketingModel.marketingLists.Add(marketingListModel);
            }
            else
            {
                marketingModel.listName = "Exclude_optdowns";
                marketingModel.listNameStatus = "false";

                marketingListModel = new MarketingListModel();
                marketingListModel.name = "Exclude_optdowns";
                marketingListModel.subscribeStatus = false;
                marketingModel.marketingLists.Add(marketingListModel);
            }
            if (sendWeeklyChild == true)
            {
                //marketingModel.listName = "Mad_Monday_subscribers_engaged";
                //marketingModel.listNameStatus = "true";
                marketingListModel = new MarketingListModel();
                marketingListModel.name = "Mad_Monday_subscribers_engaged";
                marketingListModel.subscribeStatus = true;
                marketingModel.marketingLists.Add(marketingListModel);

                marketingListModel = new MarketingListModel();
                marketingListModel.name = "Exclude_Mad_Monday-bh";
                marketingListModel.subscribeStatus = false;
                marketingModel.marketingLists.Add(marketingListModel);
            }
            else if (unsubscribeChild == true)
            {
                //marketingModel.listName = "Exclude_Mad_Monday-bh";
                //marketingModel.listNameStatus = "true";
                marketingListModel = new MarketingListModel();
                marketingListModel.name = "Mad_Monday_subscribers_engaged";
                marketingListModel.subscribeStatus = false;
                marketingModel.marketingLists.Add(marketingListModel);

                marketingListModel = new MarketingListModel();
                marketingListModel.name = "Exclude_Mad_Monday-bh";
                marketingListModel.subscribeStatus = true;
                marketingModel.marketingLists.Add(marketingListModel);
            }
            var responseMaster = marketingClient.updateMarketingContact(marketingModel, _gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password);
            
            json = JsonConvert.DeserializeObject<Object>(responseMaster);
            if (json.success != true)
            {
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = "something went wrong with your submission";

                return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
            }


            //--Master Child Call------------------------//
            //marketingModel.website = WebsiteMaster.ToString();
            //if (unsubscribeMaster == true)
            //{
            //    marketingModel.subscribeStatus = "unsubscribed";
            //    marketingModel.invalid = "true";
            //    marketingModel.forceSubscribe = "false";
            //}
            //else
            //{
            //    marketingModel.subscribeStatus = "subscribed";
            //    marketingModel.invalid = "false";
            //    marketingModel.forceSubscribe = "true";
            //}
            //if (sendWeeklyChild == true)
            //{
            //    marketingModel.listName = "Mad_Monday_subscribers_engaged";
            //    marketingModel.listNameStatus = "true";
            //}
            //else if (unsubscribeChild == true)
            //{
            //    marketingModel.listName = "Exclude_Mad_Monday-bh";
            //    marketingModel.listNameStatus = "true";
            //}
            //var responseMasterChild = marketingClient.updateMarketingContact(marketingModel, _gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password);

            //json = JsonConvert.DeserializeObject<Object>(responseMasterChild);
            //if (json.success != true)
            //{
            //    var modelError = new EmailPreferencesErrorModel();
            //    modelError.ErrorMessage = "something went wrong with your submission";

            //    return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
            //}


            //--Partner Call-----------------------//
            marketingModel.website = WebsitePartner.ToString();
            if (unsubscribePartner == true)
            {
                marketingModel.subscribeStatus = "unsubscribed";
                marketingModel.invalid = "true";
                marketingModel.forceSubscribe = "false";
            }
            else
            {
                marketingModel.subscribeStatus = "subscribed";
                marketingModel.invalid = "false";
                marketingModel.forceSubscribe = "true";
            }
            if (sendMonthlyPartner == true)
            {
                marketingModel.listName = "Exclude_optdowns";
                marketingModel.listNameStatus = "true";
                marketingListModel.name = "Exclude_optdowns";
                marketingListModel.subscribeStatus = true;
                marketingModel.marketingLists.Add(marketingListModel);
            }
            else
            {
                marketingModel.listName = "Exclude_optdowns";
                marketingModel.listNameStatus = "false";
                marketingListModel.name = "Exclude_optdowns";
                marketingListModel.subscribeStatus = false;
                marketingModel.marketingLists.Add(marketingListModel);
            }
            var responsePartner = marketingClient.updateMarketingContact(marketingModel, _gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password);

            json = JsonConvert.DeserializeObject<Object>(responsePartner);
            if (json.success != true)
            {
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = "something went wrong with your submission";

                return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
            }





            return View("~/Plugins/Widgets.Marketing/Views/EmailPrefThankyou.cshtml");
        }
    }
}
