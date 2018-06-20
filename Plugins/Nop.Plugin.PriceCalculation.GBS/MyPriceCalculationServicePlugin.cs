using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.PriceCalculation.GBS
{
    public class MyPriceCalculationServicePlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public MyPriceCalculationServicePlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/PriceCalculationConfigurationGBS/Configure";
        }     
    }
}
