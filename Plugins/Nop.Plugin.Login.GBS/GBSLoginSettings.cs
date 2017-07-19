using Nop.Core.Configuration;

namespace Nop.Plugin.Login.GBS
{
    public class GBSLoginSettings : ISettings
    {
        
        public string GBSLoginWebServiceAddress { get; set; }
        public string GBSUpdateCustomerWebService { get; set; }
        public string GBSRegisterWebService { get; set; }
        public string GBSCustomerWebServiceUserName { get; set; }
        public string GBSCustomerWebServicePassword { get; set; }
        //public string OldPassword { get; set; }


    }
}
