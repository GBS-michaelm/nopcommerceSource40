using Nop.Core.Configuration;

namespace Nop.Plugin.Order.GBS
{
    public class GBSOrderSettings : ISettings
    {
        
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string GBSOrderWebServiceAddress { get; set; }
        public string GBSPrintFileWebServiceAddress { get; set; }
        public string GBSPrintFileWebServiceBaseAddress { get; set; }
        public string GBSStoreNamePrepend { get; set; }


    }
}
