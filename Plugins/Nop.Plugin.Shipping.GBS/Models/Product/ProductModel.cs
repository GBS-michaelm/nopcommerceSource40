using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Shipping.GBS.Models.Product
{
    public class ProductGBSModel
    {
        public int ProductID { get; set; }
        public string ShippingCategoryA { get; set; }
        public string ShippingCategoryB { get; set; }
    }
}