using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.Logging.GBS
{
    public class GBSLoggingPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public GBSLoggingPlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/LoggingConfigurationGBS/Configure";
        }        
    }
}
