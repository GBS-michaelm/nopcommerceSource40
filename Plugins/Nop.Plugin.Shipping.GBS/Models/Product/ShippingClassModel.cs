using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Shipping.GBS.Models.Product
{
    public class ShippingClassModel
    {
        public int ShippingClassID { get; set; }
        public Nullable<decimal> ShippingValue { get; set; }
    }
}