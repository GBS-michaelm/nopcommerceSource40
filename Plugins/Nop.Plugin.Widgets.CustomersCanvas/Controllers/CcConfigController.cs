using Nop.Core;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers
{
    public class CcConfigController : BasePluginController
    {
        #region fields
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;
        private readonly ICcService _ccService;
        private readonly ILogger _logger;
        #endregion

        public const string CustomersCanvasApiKeyHeader = "X-CustomersCanvasAPIKey";

        #region ctor
        public CcConfigController(IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            ICcService ccService,
            ILogger logger)
        {
            _workContext = workContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _storeService = storeService;
            _ccService = ccService;
            _logger = logger;
        }
        #endregion

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>(storeScope);
            var model = new CcSettingsViewModel
            {
                ServerHostUrl = customersCanvasSettings.ServerHostUrl,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.ServerHostUrl_OverrideForStore = _settingService.SettingExists(customersCanvasSettings,
                    x => x.ServerHostUrl, storeScope);
            }
            return View("~/Plugins/Widgets.CustomersCanvas/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(CcSettingsViewModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>(storeScope);
            customersCanvasSettings.ServerHostUrl = model.ServerHostUrl;

            if (model.ServerHostUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(customersCanvasSettings, x => x.ServerHostUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(customersCanvasSettings, x => x.ServerHostUrl, storeScope);

            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }



        #region upload fonts

        // Add security key to web.config in NopCommerce (Nop.Web):
        // <appSettings>
        //    <add key = "CustomersCanvasApiSecurityKey" value="MYSECURITYKEY"/>
        // </appSettings>
        [HttpPost]
        public ActionResult ReloadLocalFonts()
        {
            var CustomersCanvasSecurityKey = System.Configuration.ConfigurationManager.AppSettings["CustomersCanvasApiSecurityKey"];
            
            if (CustomersCanvasSecurityKey == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,
                    _localizationService.GetResource("Plugins.Widgets.CustomersCanvas.CustomersCanvasApiKeyWarning"));
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[CustomersCanvasApiKeyHeader] = CustomersCanvasSecurityKey;
                    var updateFontLink = _ccService.ServerHostUrl() + "/api/FontPreview/UpdateLocalFonts";
                    client.UploadString(updateFontLink, "POST", "");
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion
    }
}
