using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcSettingsViewModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.Config.ServerHostUrl")]
        public string ServerHostUrl { get; set; }
        public bool ServerHostUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.Config.CCApiKey")]
        public string CustomersCanvasApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.Config.DesignFileName")]
        public string DesignFileName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.Config.IsOrderExportButton")]
        public bool IsOrderExportButton { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.Config.OrderExportPath")]
        public string OrderExportPath { get; set; }
    }
}
