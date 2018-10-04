using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Shipping.GBS
{
    public class GBSShippingSetting : ISettings
    {

        public string LoginId { get; set; }
        public string Password { get; set; }
        public string GBSShippingWebServiceAddress { get; set; }
        public string GBSStoreNamePrepend { get; set; }
        public bool UseFlatRate { get; set; }
        public decimal FlatRateAmount { get; set; }


    }
}