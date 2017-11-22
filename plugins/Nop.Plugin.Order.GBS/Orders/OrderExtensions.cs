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
using Nop.Plugin.DataAccess.GBS;
using Nop.Core;
using Nop.Core.Domain.Customers;
using System.Data;
using Newtonsoft.Json;

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
                Core.Domain.Orders.Order order = EngineContext.Current.Resolve<IOrderService>().GetOrderById(orderId);
                //string orderSON = JsonConvert.SerializeObject(order, Formatting.Indented,
                //                new JsonSerializerSettings()
                //                {
                //                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                //                });
                return order;
            }else
            {
                //Nop.Core.Domain.Orders.Order legacyOrder = new Nop.Core.Domain.Orders.Order();
                //legacyOrder.Id = 999999; legacyOrder
                //setCustomValue("isLegacy", true, legacyOrder);
                //IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
                //Customer customer = _workContext.CurrentCustomer;
                //legacyOrder.Customer = customer;
                //legacyOrder.CustomerId = customer.Id;
                //legacyOrder.OrderTotal = Decimal.Parse("3.95");
                //var _productService = EngineContext.Current.Resolve<IProductService>();
                //var product = _productService.GetProductBySku("NCCPH6-00004");
                //var orderItem = new Core.Domain.Orders.OrderItem()
                //{
                //    Id = 999999,
                //    Product = product,
                //    ProductId = product.Id,
                //    Quantity = 1,
                //    OrderItemGuid = new Guid(),
                //    AttributeDescription = "attribute stuff",
                //    UnitPriceExclTax = product.Price

                //};
                //legacyOrder.OrderItems.Add(orderItem);

                GBSOrderSettings _gbsOrderSettings = EngineContext.Current.Resolve<GBSOrderSettings>();
                Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                paramDicEx.Add("@orderId", orderId);

                DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);
                string select = "EXEC usp_getLegacyOrderForNOP @orderId";
                DataView orderResult = manager.GetParameterizedDataView(select, paramDicEx);

                var legacyOrder = JsonConvert.DeserializeObject<Nop.Core.Domain.Orders.Order>((string)orderResult[0][0]);
                var _productService = EngineContext.Current.Resolve<IProductService>();
                foreach(var item in legacyOrder.OrderItems)
                {
                    var product = _productService.GetProductBySku(item.Product.Sku);
                    if (product != null)
                    {
                        item.Product = product;
                        item.ProductId = product.Id;
                    }
                }
                return legacyOrder;
            }
        }

        public static List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> getLegacyOrders()
        {
            // Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel myOrder = new Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel();
            // myOrder.Id = 99999999;
            // //myOrder.CustomValues["isLegacy"] = true;
            // //OrderDetailsModel.OrderItemModel myItem = new OrderDetailsModel.OrderItemModel();
            // //myItem.Id = 11111111;
            // //myItem.ProductId = _productService.GetProductBySku("NCALH6-00001").Id;
            // //bool isLegacy = Convert.ToBoolean(OrderExtensions.getCustomValue("isLegacy", myOrder));
            // myOrder.CustomProperties["isLegacy"] = true;

            //// myOrder.Items.Add(myItem);
            // List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> myOrders = new List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel>();
            // myOrders.Add(myOrder);

            GBSOrderSettings _gbsOrderSettings = EngineContext.Current.Resolve<GBSOrderSettings>();
            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
            Customer customer = _workContext.CurrentCustomer;
            Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
            paramDicEx.Add("@email", customer.Email);
            paramDicEx.Add("@website", _gbsOrderSettings.GBSStoreNamePrepend);

            DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);
            string select = "EXEC usp_getLegacyOrdersForNOP @email, @website";
            DataView orderItemResult = manager.GetParameterizedDataView(select, paramDicEx);

            var myOrders = JsonConvert.DeserializeObject<List<CustomerOrderListModel.OrderDetailsModel>>((string)orderItemResult[0][0]);
            return myOrders;
        }
    }
}
