using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcSettingsViewModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.ServerHostUrl")]
        [AllowHtml]
        public string ServerHostUrl { get;  set; }
        public bool ServerHostUrl_OverrideForStore { get; set; }
    }
}
