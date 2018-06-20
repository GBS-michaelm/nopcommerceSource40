using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.Login.GBS
{
    public class MyLoginServicePlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public MyLoginServicePlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/LoginConfigurationGBS/Configure";
        }       
    }
}
