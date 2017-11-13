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

                if (modelError.ErrorMessage != "record not found")
                {
                    return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
                }
            }

            var model = new EmailPreferencesModel();
            model.ContactPref = marketingContact;
            model.subscribeStatusMaster = model.ContactPref.subscribeStatus;

            if (marketingContact.errorMessage == "record not found")
            {
                model.ContactPref.emailAddress = Email;
            }
            else
            {
                model.masterFound = true;
            }

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

                    foreach (MarketingListModel item in model.ContactPref.marketingLists)
                    {
                        if (item.name == getList.SubListName)
                        {
                            getList.listSubscribeStatus = true;
                            getList.listFound = true;
                        }
                        else if (item.name == getList.UnSubListName)
                        {
                            getList.listSubscribeStatus = false;
                            getList.listFound = true;
                        }
                    }

                    model.ShowLists.Add(getList);
                }
            }

            MarketingContactModel marketingContact2 = marketingClient.getMarketingContact(_gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password, Email, "NCC");
            if (marketingContact2.error)
            {
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = marketingContact2.errorMessage;

                if (modelError.ErrorMessage != "record not found")
                {
                    return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
                }
            }

            model.ContactPref = marketingContact2;
            model.subscribeStatusPartner = model.ContactPref.subscribeStatus;

            if (marketingContact2.errorMessage == "record not found")
            {
                model.ContactPref.emailAddress = Email;
            }
            else
            {
                model.partnerFound = true;
            }

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

                    foreach (MarketingListModel item in model.ContactPref.marketingLists)
                    {
                        if (item.name == getList2.SubListName)
                        {
                            getList2.listSubscribeStatus = true;
                            getList2.listFound = true;
                        }
                        else if (item.name == getList2.UnSubListName)
                        {
                            getList2.listSubscribeStatus = false;
                            getList2.listFound = true;
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

            //--Setup Master-------------//
            string MasterID = form["MasterID"].ToString();
            bool processMaster = true;
            bool unsubscribeMaster = false;
            bool sendMonthlyMaster = false;
            if (Request.Form[MasterID] != null)
            {
                string SelectedListMaster = form[MasterID].ToString();

                if (SelectedListMaster == "sendmonthly")
                {
                    sendMonthlyMaster = true;
                }
                else if (SelectedListMaster == "unsubscribe")
                {
                    unsubscribeMaster = true;
                }
            }
            else
            {
                processMaster = false;
            }
            //----------------------------------//

            //--Setup Master Child-------------//
            string ChildID = form["ChildID"].ToString();
            bool processChild = true;
            bool unsubscribeChild = false;
            bool sendWeeklyChild = false;
            if (Request.Form[ChildID] != null)
            {
                string SelectedListChild = form[ChildID].ToString();
                
                if (SelectedListChild == "sendweekly")
                {
                    sendWeeklyChild = true;
                }
                else if (SelectedListChild == "unsubscribe")
                {
                    unsubscribeChild = true;
                }
            }
            else
            {
                processChild = false;
            }
            //----------------------------------//

            //--Setup Partner-------------//
            string MasterPartnerID = form["MasterPartnerID"].ToString();
            bool processPartner = true;
            bool unsubscribePartner = false;
            bool sendMonthlyPartner = false;
            if (Request.Form[MasterPartnerID] != null)
            {
                string SelectedListPartner = form[MasterPartnerID].ToString();

                if (SelectedListPartner == "sendmonthly")
                {
                    sendMonthlyPartner = true;
                }
                else if (SelectedListPartner == "unsubscribe")
                {
                    unsubscribePartner = true;
                }
            }
            else
            {
                processPartner = false;
            }
            //----------------------------------//

            //--Setup Partner Child-------------//
            //string ChildPartnerID = form["ChildPartnerID"].ToString();
            //string SelectedListChildPartner = form[ChildPartnerID].ToString();
            //bool unsubscribeChildPartner = false;
            //bool sendWeeklyChildPartner = false;
            //if (SelectedListChildPartner == "sendweekly")
            //{
            //    sendWeeklyChildPartner = true;
            //}
            //else if (SelectedListChildPartner == "unsubscribe")
            //{
            //    unsubscribeChildPartner = true;
            //}
            //----------------------------------//


            //--Master Call------------------------//
            if (processMaster == true)
            {
                GBSMarketingServiceClient marketingClient = new GBSMarketingServiceClient();
                MarketingContactModel marketingModel = new MarketingContactModel();
                MarketingListModel marketingListModel = new MarketingListModel();
                dynamic json = null;
                marketingModel.emailAddress = EmailAddress.ToString();
                marketingModel.providerId = EmailAddress.ToString();
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
                    marketingListModel = new MarketingListModel();
                    marketingListModel.name = "Exclude_optdowns";
                    marketingListModel.subscribeStatus = "true";
                    marketingModel.marketingLists.Add(marketingListModel);
                }
                else
                {
                    marketingListModel = new MarketingListModel();
                    marketingListModel.name = "Exclude_optdowns";
                    marketingListModel.subscribeStatus = "false";
                    marketingModel.marketingLists.Add(marketingListModel);
                }

                if (processChild == true)
                {
                    if (sendWeeklyChild == true)
                    {
                        marketingListModel = new MarketingListModel();
                        marketingListModel.name = "Mad_Monday_subscribers_engaged";
                        marketingListModel.subscribeStatus = "true";
                        marketingModel.marketingLists.Add(marketingListModel);

                        marketingListModel = new MarketingListModel();
                        marketingListModel.name = "Exclude_Mad_Monday-bh";
                        marketingListModel.subscribeStatus = "false";
                        marketingModel.marketingLists.Add(marketingListModel);
                    }
                    else if (unsubscribeChild == true)
                    {
                        marketingListModel = new MarketingListModel();
                        marketingListModel.name = "Mad_Monday_subscribers_engaged";
                        marketingListModel.subscribeStatus = "false";
                        marketingModel.marketingLists.Add(marketingListModel);

                        marketingListModel = new MarketingListModel();
                        marketingListModel.name = "Exclude_Mad_Monday-bh";
                        marketingListModel.subscribeStatus = "true";
                        marketingModel.marketingLists.Add(marketingListModel);
                    }
                }

                var responseMaster = marketingClient.updateMarketingContact(marketingModel, _gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password);

                json = JsonConvert.DeserializeObject<Object>(responseMaster);
                if (json.success != true)
                {
                    var modelError = new EmailPreferencesErrorModel();
                    modelError.ErrorMessage = "something went wrong with your submission";

                    return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError);
                }
            }


            //--Partner Call-----------------------//
            if (processPartner == true)
            {
                GBSMarketingServiceClient marketingClient2 = new GBSMarketingServiceClient();
                MarketingContactModel marketingModel2 = new MarketingContactModel();
                MarketingListModel marketingListPartnerModel = new MarketingListModel();
                dynamic json2 = null;
                marketingModel2.emailAddress = EmailAddress.ToString();
                marketingModel2.providerId = EmailAddress.ToString();
                marketingModel2.website = WebsitePartner.ToString();
                //marketingModel2.marketingLists.Clear();
                if (unsubscribePartner == true)
                {
                    marketingModel2.subscribeStatus = "unsubscribed";
                    marketingModel2.invalid = "true";
                    marketingModel2.forceSubscribe = "false";
                }
                else
                {
                    marketingModel2.subscribeStatus = "subscribed";
                    marketingModel2.invalid = "false";
                    marketingModel2.forceSubscribe = "true";
                }
                if (sendMonthlyPartner == true)
                {
                    marketingListPartnerModel = new MarketingListModel();
                    marketingListPartnerModel.name = "Exclude_optdowns";
                    marketingListPartnerModel.subscribeStatus = "true";
                    marketingModel2.marketingLists.Add(marketingListPartnerModel);
                }
                else
                {
                    marketingListPartnerModel = new MarketingListModel();
                    marketingListPartnerModel.name = "Exclude_optdowns";
                    marketingListPartnerModel.subscribeStatus = "false";
                    marketingModel2.marketingLists.Add(marketingListPartnerModel);
                }

                //if (sendWeeklyChildPartner == true)
                //{
                //    marketingListPartnerModel = new MarketingListModel();
                //    marketingListPartnerModel.name = "Mad_Monday_subscribers_engaged";
                //    marketingListPartnerModel.subscribeStatus = "true";
                //    marketingModel.marketingLists.Add(marketingListPartnerModel);

                //    marketingListPartnerModel = new MarketingListModel();
                //    marketingListPartnerModel.name = "Exclude_Mad_Monday-bh";
                //    marketingListPartnerModel.subscribeStatus = "false";
                //    marketingModel.marketingLists.Add(marketingListPartnerModel);
                //}
                //else if (unsubscribeChildPartner == true)
                //{
                //    marketingListPartnerModel = new MarketingListModel();
                //    marketingListPartnerModel.name = "Mad_Monday_subscribers_engaged";
                //    marketingListPartnerModel.subscribeStatus = "false";
                //    marketingModel.marketingLists.Add(marketingListPartnerModel);

                //    marketingListPartnerModel = new MarketingListModel();
                //    marketingListPartnerModel.name = "Exclude_Mad_Monday-bh";
                //    marketingListPartnerModel.subscribeStatus = "true";
                //    marketingModel.marketingLists.Add(marketingListPartnerModel);
                //}



                var responsePartner = marketingClient2.updateMarketingContact(marketingModel2, _gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password);

                json2 = JsonConvert.DeserializeObject<Object>(responsePartner);
                if (json2.success != true)
                {
                    var modelError2 = new EmailPreferencesErrorModel();
                    modelError2.ErrorMessage = "something went wrong with your submission";

                    return View("~/Plugins/Widgets.Marketing/Views/EmailPrefError.cshtml", modelError2);
                }
            }


            return View("~/Plugins/Widgets.Marketing/Views/EmailPrefThankyou.cshtml");
        }
    }
}
