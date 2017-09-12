using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Order.GBS.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
                
        [NopResourceDisplayName("Plugins.Order.GBS.Fields.LoginId")]
        public string LoginId { get; set; }
        public bool LoginId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.GBS.Fields.Password")]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.GBS.Fields.GBSOrderWebServiceAddress")]
        public string GBSOrderWebServiceAddress { get; set; }
        public bool GBSOrderWebServiceAddress_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.GBS.Fields.GBSPrintFileWebServiceAddress")]
        public string GBSPrintFileWebServiceAddress { get; set; }
        public bool GBSPrintFileWebServiceAddress_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.GBS.Fields.GBSPrintFileWebServiceBaseAddress")]
        public string GBSPrintFileWebServiceBaseAddress { get; set; }
        public bool GBSPrintFileWebServiceBaseAddress_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.GBS.Fields.GBSStoreNamePrepend")]
        public string GBSStoreNamePrepend { get; set; }
        public bool GBSStoreNamePrepend_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Order.GBS.Fields.HOMConnectionString")]
        public string HOMConnectionString { get; set; }
        public bool HOMConnectionString_OverrideForStore { get; set; }
    }
}