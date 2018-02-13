﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Nop.Services;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Core.Domain.Localization;
using Nop.Core.Data;
using Nop.Core;
using Nop.Web.Models.Common;
using Nop.Core.Caching;
using Nop.Web.Infrastructure.Cache;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Marketing.EmailPref.Models;
using GBSMarketingService;
using WebServices.Models;
using Nop.Services.Logging;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.Marketing.Controllers
{
    public class WidgetsMarketingController : BasePluginController
    {
        private readonly GBSMarketingSettings _gbsMarketingSettings;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public WidgetsMarketingController(
            GBSMarketingSettings gbsMarketingSettings,
            IWorkContext workContext,
            ILogger logger)
        {
            this._gbsMarketingSettings = gbsMarketingSettings;
            this._workContext = workContext;
            this._logger = logger;
        }

        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {

            return View("~/Plugins/Widgets.Marketing/Views/PublicInfo.cshtml");
        }

        public ActionResult JoinListForm(string widgetZone, object additionalData = null)
        {
            //pass view name from widget call in additionalData to specify which view to load
            dynamic o = JsonConvert.DeserializeObject<Object>(JsonConvert.SerializeObject(additionalData));
            //load forms from Theme
            MarketingFormModel model = new MarketingFormModel()
            {
                EmailList = o.list,
                Accounts = o.accounts
            };
            return View((string)o.form, model);
        }

        public ActionResult JoinList (FormCollection form)
        {
            try
            {
                GBSMarketingServiceClient marketingClient = new GBSMarketingServiceClient();
                MarketingContactModel marketingModel = new MarketingContactModel();
                MarketingListModel marketingListModel = new MarketingListModel();
                dynamic json = null;
                marketingModel.emailAddress = form["emailAddress"];
                marketingModel.providerId = form["emailAddress"];
                marketingModel.website = form["accounts"];

                marketingModel.subscribeStatus = "subscribed";
                marketingModel.invalid = "false";
                marketingModel.forceSubscribe = "true";



                marketingListModel = new MarketingListModel();
                marketingListModel.name = form["emailLists"];
                marketingListModel.subscribeStatus = "true";
                marketingModel.marketingLists.Add(marketingListModel);




                var responseMaster = marketingClient.updateMarketingContact(marketingModel, _gbsMarketingSettings.GBSMarketingWebServiceAddress, _gbsMarketingSettings.LoginId, _gbsMarketingSettings.Password);

                json = JsonConvert.DeserializeObject<Object>(responseMaster);
                if (json.success != true)
                {
                    var modelError = new EmailPreferencesErrorModel();
                    modelError.ErrorMessage = "something went wrong with your submission";

                    return View("~/Plugins/Widgets.Marketing/Views/JoinListError.cshtml", modelError);
                }

                return View("~/Plugins/Widgets.Marketing/Views/JoinListThankYou.cshtml");
            }catch (Exception ex)
            {
                _logger.Error("Error in JoinList: Join Email = " + form["emailAddress"] + " Lists = " + form["emailLists"], ex, _workContext.CurrentCustomer);
                var modelError = new EmailPreferencesErrorModel();
                modelError.ErrorMessage = "something went wrong with your submission";
                return View("~/Plugins/Widgets.Marketing/Views/JoinListError.cshtml", modelError);
            }
        }

    }
}
