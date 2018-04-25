using System.Web.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Widgets.MarketCenters.GBS.Models;
using System;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Common;
using System.Linq;
using System.Web;
using System.IO;
using Nop.Plugin.Widgets.MarketCenters.GBS.Services;
using Nop.Plugin.Widgets.MarketCenters.GBS.Domain;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Security;
using System.Collections.Generic;
using Nop.Services.Catalog;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using Nop.Services.Logging;
using Nop.Plugin.Widgets.MarketCenters.GBS.Models.Clients;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Controllers
{
    public class MarketCentersController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;        
        private readonly IPermissionService _permissionService;
        private readonly IPriceFormatter _priceFormatter;

        private readonly CaptchaSettings _captchaSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly EmailAccountSettings _emailAccountSettings;

        private readonly IMarketCentersService _marketCentersService;
        private readonly IStoreService _storeService;

        public MarketCentersController(ICustomerService customerService,
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,            
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IStoreContext storeContext,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            CaptchaSettings captchaSettings,
            EmailAccountSettings emailAccountSettings,
            IMarketCentersService marketCentersService,
            IStoreService storeService)
        {
            this._customerService = customerService;
            this._customerSettings = customerSettings;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;            
            this._permissionService = permissionService;
            this._priceFormatter = priceFormatter;

            this._storeContext = storeContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._captchaSettings = captchaSettings;
            this._emailAccountSettings = emailAccountSettings;

            this._marketCentersService = marketCentersService;
            this._storeService = storeService;
        }

        [AdminAuthorize]        
        // Show the list of startup customers stored in external table
        public ActionResult Index()
        {
            var model = new ClientListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public ActionResult ClientList(DataSourceRequest command)
        {            
            var clients = _marketCentersService.GetClients();
            var gridModel = new DataSourceResult
            {
                Data = clients,
                Total = clients.Count()
            };
            return Json(gridModel);
        }

        [AdminAuthorize]
        public ActionResult CreateClient()
        {
            var model = new ClientDetailsModel();
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/CreateClient.cshtml", model);
        }

        [AdminAuthorize]
        public ActionResult EditClient()
        {
            var model = new ClientDetailsModel();
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/CreateClient.cshtml",model);
        }

        public ActionResult PublicInfo()
        {
            return new EmptyResult();
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Contacts()
        {
            ContactModel model = new ContactModel();
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/_Contact.cshtml", model);
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult ApprovedProducts()
        {
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/_ApprovedProducts.cshtml");
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Proofs()
        {
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/_Proofs.cshtml");
        }

        [AdminAuthorize]        
        public ActionResult AssociateSpecificBacks()
        {
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/AssociateSpecificBacks.cshtml");
        }

        [AdminAuthorize]        
        public ActionResult CustomizeDesign()
        {
            StyleGuideModel model = new StyleGuideModel();
            model.PrimaryColor = "#f8d905";
            model.SecondaryColor = "#f1e196";
            model.AccentColor = "#51b016";
            model.LightBodyColor = "#ffffff";
            model.DarkBodyColor = "#000000";
            model.LightBackground = "#000000";
            model.DarkBackground = "#fff";
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/CustomizeDesign.cshtml",model);
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult StyleGuide()
        {
            StyleGuideModel model = new StyleGuideModel();
            model.PrimaryColor = "#f8d905";
            model.SecondaryColor = "#f1e196";
            model.AccentColor = "#51b016";
            model.LightBodyColor = "#ffffff";
            model.DarkBodyColor = "#000000";
            model.LightBackground = "#000000";
            model.DarkBackground = "#fff";
            return View("~/Plugins/Widgets.MarketCenters.GBS/Views/_StyleGuide.cshtml", model);
        }

    }
}