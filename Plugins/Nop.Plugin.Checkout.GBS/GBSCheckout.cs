using Nop.Core.Plugins;
using Nop.Services.Common;
using System;

namespace Nop.Plugin.Checkout.GBS
{
    public class GBSCheckout : BasePlugin, IMiscPlugin
    {
        public override string GetConfigurationPageUrl()
        {
            return "";
        }       
    }
}
