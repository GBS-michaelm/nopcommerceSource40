using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Admin.Models.Orders;
using Nop.Plugin.Widgets.CustomersCanvas.Data;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcOrderModel
    {
        public List<Core.Domain.Orders.OrderItem> Items { get; set; }
        public List<CcResult> CcResult { get; set; }
        public CcOrderModel()
        {
            Items = new List<Core.Domain.Orders.OrderItem>();
            CcResult = new List<Data.CcResult>();
        }
    }
}
