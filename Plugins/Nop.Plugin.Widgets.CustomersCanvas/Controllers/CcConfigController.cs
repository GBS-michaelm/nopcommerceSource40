using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Net;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers
{
    [Area(AreaNames.Admin)]
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
        public CcConfigController(
            IStoreService storeService,
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

        [AuthorizeAdmin]
        public ActionResult Configure()
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>(storeScope);
            var model = new CcSettingsViewModel
            {
                ServerHostUrl = customersCanvasSettings.ServerHostUrl,
                CustomersCanvasApiKey = customersCanvasSettings.CustomersCanvasApiKey,
                ActiveStoreScopeConfiguration = storeScope,
                DesignFileName = customersCanvasSettings.DesignFileName,
                IsOrderExportButton = customersCanvasSettings.IsOrderExportButton,
                OrderExportPath = customersCanvasSettings.OrderExportPath
            };
            if (storeScope > 0)
            {
                model.ServerHostUrl_OverrideForStore = _settingService.SettingExists(customersCanvasSettings,
                    x => x.ServerHostUrl, storeScope);
            }
            return View("~/Plugins/Widgets.CustomersCanvas/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult Configure(CcSettingsViewModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>(storeScope);
            customersCanvasSettings.ServerHostUrl = model.ServerHostUrl;
            if (model.ServerHostUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(customersCanvasSettings, x => x.ServerHostUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(customersCanvasSettings, x => x.ServerHostUrl, storeScope);

            customersCanvasSettings.DesignFileName = model.DesignFileName;
            _settingService.SaveSetting(customersCanvasSettings, x => x.DesignFileName, storeScope, false);

            customersCanvasSettings.IsOrderExportButton = model.IsOrderExportButton;
            _settingService.SaveSetting(customersCanvasSettings, x => x.IsOrderExportButton, storeScope, false);

            customersCanvasSettings.CustomersCanvasApiKey = model.CustomersCanvasApiKey;
            _settingService.SaveSetting(customersCanvasSettings, x => x.CustomersCanvasApiKey, storeScope, false);

            if (!model.IsOrderExportButton)
            {
                model.OrderExportPath = "";
            }
            customersCanvasSettings.OrderExportPath = model.OrderExportPath;
            _settingService.SaveSetting(customersCanvasSettings, x => x.OrderExportPath, storeScope, false);

            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        #region upload fonts

        [HttpPost]
        public IActionResult ReloadLocalFonts()
        {
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>();
            string CustomersCanvasSecurityKey = customersCanvasSettings.CustomersCanvasApiKey;
            //System.Configuration.ConfigurationManager.AppSettings["CustomersCanvasApiSecurityKey"];

            if (string.IsNullOrEmpty(CustomersCanvasSecurityKey))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    _localizationService.GetResource("Plugins.Widgets.CustomersCanvas.Config.CustomersCanvasApiKeyWarning"));
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
                return StatusCode((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion
    }
}
