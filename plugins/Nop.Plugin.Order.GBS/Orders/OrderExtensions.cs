using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Payments;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Orders;
using Nop.Web.Models.Order;
using Nop.Services.Catalog;

namespace Nop.Plugin.Order.GBS.Orders
{
    public static class OrderExtensions
    {
        public static IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        public static object getCustomValue(string key, Nop.Core.Domain.Orders.Order order)
        {
            Dictionary<string, object> customValues = PaymentExtensions.DeserializeCustomValues(order);

            return customValues[key];
        }

        public static void setCustomValue(string key, object value, Nop.Core.Domain.Orders.Order order)
        {
            Dictionary<string, object> customValues = PaymentExtensions.DeserializeCustomValues(order);
            customValues[key] = value;
            ProcessPaymentRequest request = new ProcessPaymentRequest();
            request.CustomValues = customValues;
            order.CustomValuesXml = request.SerializeCustomValues();

        }
        public static Nop.Core.Domain.Orders.Order GetOrderById(int orderId, bool isLegacy)
        {
            if (!isLegacy)
            {
                return EngineContext.Current.Resolve<IOrderService>().GetOrderById(orderId);
            }else
            {
                Nop.Core.Domain.Orders.Order legacyOrder = new Nop.Core.Domain.Orders.Order();
                var _productService = EngineContext.Current.Resolve<IProductService>(); ;
                return legacyOrder;
            }
        }

        public static List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> getLegacyOrders()
        {
            Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel myOrder = new Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel();
            myOrder.Id = 99999999;
            //myOrder.CustomValues["isLegacy"] = true;
            //OrderDetailsModel.OrderItemModel myItem = new OrderDetailsModel.OrderItemModel();
            //myItem.Id = 11111111;
            //myItem.ProductId = _productService.GetProductBySku("NCALH6-00001").Id;
            //bool isLegacy = Convert.ToBoolean(OrderExtensions.getCustomValue("isLegacy", myOrder));
            myOrder.CustomProperties["isLegacy"] = true;

           // myOrder.Items.Add(myItem);
            List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> myOrders = new List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel>();
            myOrders.Add(myOrder);
            return myOrders;
        }
    }
}
