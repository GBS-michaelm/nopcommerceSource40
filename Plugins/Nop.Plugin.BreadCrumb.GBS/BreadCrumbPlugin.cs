using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.BreadCrumb.GBS
{
    public class BreadCrumbPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public BreadCrumbPlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/BreadCrumbGBS/Configure";
        }
    }
}
