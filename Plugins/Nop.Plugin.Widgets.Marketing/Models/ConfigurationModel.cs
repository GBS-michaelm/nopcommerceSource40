using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.Marketing.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
                
        [NopResourceDisplayName("Plugins.Widgets.Marketing.Fields.LoginId")]
        public string LoginId { get; set; }
        public bool LoginId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Marketing.Fields.Password")]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.Widgets.Marketing.GBSMarketingWebServiceAddress")]
        public string GBSMarketingWebServiceAddress { get; set; }
        public bool GBSMarketingWebServiceAddress_OverrideForStore { get; set; }

    }
}