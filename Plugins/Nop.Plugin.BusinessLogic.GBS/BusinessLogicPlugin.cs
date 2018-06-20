using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.BusinessLogic.GBS
{
    public class BusinessLogicPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public BusinessLogicPlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ConfigurationBusinessGBS/Configure";
        }
    }
}
