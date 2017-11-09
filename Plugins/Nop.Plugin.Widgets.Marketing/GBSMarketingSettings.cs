using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Marketing
{
    public class GBSMarketingSettings : ISettings
    {
        
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string GBSMarketingWebServiceAddress { get; set; }

    }
}
