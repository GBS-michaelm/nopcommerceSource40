using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "AdminOrderListButtons")]
    public class AdminOrderListButtonsViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;

        public AdminOrderListButtonsViewComponent(
            ISettingService settingService)
        {
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>();
            if (customersCanvasSettings.IsOrderExportButton)
            {
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Order/AdminList.cshtml");
            }
            return Content("");
        }

    }
}
