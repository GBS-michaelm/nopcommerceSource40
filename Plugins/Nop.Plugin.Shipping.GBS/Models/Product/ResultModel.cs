using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Shipping.GBS.Models.Product
{
    public class ResultModel
    {
        public string zipPrefix { get; set; }
        public string shippingCategory { get; set; }
        public string  shippingClassID { get; set; }
        public Nullable<decimal> shippingValue { get; set; }

    }
}