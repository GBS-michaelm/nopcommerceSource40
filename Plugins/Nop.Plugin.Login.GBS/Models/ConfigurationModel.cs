using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Login.GBS.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
                
        [NopResourceDisplayName("Plugins.Login.GBS.Fields.GBSLoginWebServiceAddress")]
        public string GBSLoginWebServiceAddress { get; set; }
        public bool GBSLoginWebServiceAddress_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Login.GBS.Fields.GBSRegisterWebService")]
        public string GBSRegisterWebService { get; set; }
        public bool GBSRegisterWebService_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Login.GBS.Fields.GBSUpdateCustomerWebService")]
        public string GBSUpdateCustomerWebService { get; set; }
        public bool GBSUpdateCustomerWebService_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Login.GBS.Fields.GBSCustomerWebServiceUserName")]
        public string GBSCustomerWebServiceUserName { get; set; }
        public bool GBSCustomerWebServiceName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Login.GBS.Fields.GBSCustomerWebServicePassword")]
        public string GBSCustomerWebServicePassword { get; set; }
        public bool GBSCustomerWebServicePassword_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Login.GBS.Fields.OldPassword")]
        //public string GBSOldPassword { get; set; }
        //public bool GBSOldPassword_OverrideForStore { get; set; }
    }
}