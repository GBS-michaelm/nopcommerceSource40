﻿using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.PriceCalculation.GBS.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
                
        //[NopResourceDisplayName("Plugins.PriceCalculation.GBS.Fields.GBSLoginWebServiceAddress")]
        //public string GBSLoginWebServiceAddress { get; set; }
        //public bool GBSLoginWebServiceAddress_OverrideForStore { get; set; }


    }
}