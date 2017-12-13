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
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using System.Data.Common;
using System.Reflection;

namespace Nop.Plugin.Order.GBS.Orders
{
    public class OrderExtensions
    {
        public  IProductService _productService = EngineContext.Current.Resolve<IProductService>();
        public  ICcService ccService = EngineContext.Current.Resolve<ICcService>();
        public  IProductAttributeParser productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
        public  IProductAttributeService productAttributeService = EngineContext.Current.Resolve<IProductAttributeService>();
        public  GBSOrderSettings _gbsOrderSettings = EngineContext.Current.Resolve<GBSOrderSettings>();
        public ISpecificationAttributeService _specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
        public Nop.Services.Custom.Orders.GBSOrderService _gbsOrderService = (Nop.Services.Custom.Orders.GBSOrderService)EngineContext.Current.Resolve<IOrderService>();
        //public static DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);



        public  object getCustomValue(string key, Nop.Core.Domain.Orders.Order order)
        {
            Dictionary<string, object> customValues = PaymentExtensions.DeserializeCustomValues(order);

            return customValues[key];
        }

        public  void setCustomValue(string key, object value, Nop.Core.Domain.Orders.Order order)
        {
            Dictionary<string, object> customValues = PaymentExtensions.DeserializeCustomValues(order);
            customValues[key] = value;
            ProcessPaymentRequest request = new ProcessPaymentRequest();
            request.CustomValues = customValues;
            order.CustomValuesXml = request.SerializeCustomValues();

        }
        public  Nop.Core.Domain.Orders.Order GetOrderById(int orderId, bool isLegacy)
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

                //GBSOrderSettings _gbsOrderSettings = EngineContext.Current.Resolve<GBSOrderSettings>();
                Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                paramDicEx.Add("@orderId", orderId);

                DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);
                string select = "EXEC usp_getLegacyOrderForNOP @orderId";
                //DataView orderResult = manager.GetParameterizedDataView(select, paramDicEx);
                string jsonResult = manager.GetParameterizedJsonString(select, paramDicEx);

                var legacyOrder = JsonConvert.DeserializeObject<Nop.Core.Domain.Orders.Order>(jsonResult);
                //var _productService = EngineContext.Current.Resolve<IProductService>();

                foreach(var item in legacyOrder.OrderItems)
                {
                    prepareLegacyOrderItem(item);
                }
                return legacyOrder;
            }
        }

        public LegacyOrderItem ConvertToLegacyOrderItem(OrderItem orderItem)
        {
            var type = typeof(LegacyOrderItem);
            var instance = Activator.CreateInstance(type);

            if (type.BaseType != null)
            {
                var properties = type.BaseType.GetProperties();
                foreach (var property in properties)
                    if (property.CanWrite)
                        property.SetValue(instance, property.GetValue(orderItem, null), null);
            }

            return (LegacyOrderItem)instance;
        }

        public class LegacyOrderItem : Nop.Core.Domain.Orders.OrderItem
        {
            public string legacyPicturePath { get; set; }
            public string productOptionsJson { get; set; }
            public string webToPrintType { get; set; }
        }

        public  Nop.Core.Domain.Orders.OrderItem GetOrderItemById(int orderItemId, bool isLegacy)
        {
            if (!isLegacy)
            {
                Core.Domain.Orders.OrderItem orderItem = EngineContext.Current.Resolve<IOrderService>().GetOrderItemById(orderItemId);
                return orderItem;
            }
            else
            {
                DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);

                Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                paramDicEx.Add("@orderItemId", orderItemId);

                string select = "EXEC usp_getLegacyOrderItemForNOP @orderItemId";
                //DataView orderResult = manager.GetParameterizedDataView(select, paramDicEx);
                string jsonResult = manager.GetParameterizedJsonString(select, paramDicEx);

                var legacyOrderItem = JsonConvert.DeserializeObject<LegacyOrderItem>(jsonResult);
                return prepareLegacyOrderItem(legacyOrderItem);
            }
        }


        public  OrderItem prepareLegacyOrderItem(OrderItem orderItem)
        {
            //var ccId = ccService.GetCcAttrId(CcProductAttributes.CcId);
            var product = _productService.GetProductBySku(orderItem.Product.Sku);
            if (product != null)
            {
                orderItem.Product = product;
                orderItem.ProductId = product.Id;
                //var mappings = productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                //var mapping = mappings.FirstOrDefault(x => x.ProductAttributeId == ccId);
                //var attributesXml = productAttributeParser.AddProductAttribute("",
                //                mapping, null);
                //orderItem.AttributesXml = attributesXml;
            }
            //else
            //{
            //    orderItem.ProductId = -1;
            //    orderItem.Product.Id = -1;
            //}
            return orderItem;
        }
        public class TreatmentData 
        {
            public int id { get; set; }
            public string DefaultColor { get; set; }
            public string ClassName { get; set; }
        }
        public List<TreatmentData> getTreatmentData(OrderDetailsModel orderDetailsModel)
        {
            List<TreatmentData> treatmentDataList = new List<TreatmentData>();
            foreach (var orderItemModel in orderDetailsModel.Items)
            {
                TreatmentData treatmentDataItem = new TreatmentData();
                treatmentDataItem.id = orderItemModel.Id;
                var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(orderItemModel.ProductId);
                var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);
                if (imageSpecAttrOption.Any())
                {

                    var thumbnailBackground = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Treatment");
                    if (thumbnailBackground.Any())
                    {
                        foreach (var thbackground in thumbnailBackground)
                        {
                            var gbsBackGroundName = thbackground.Name;
                            treatmentDataItem.DefaultColor = "";
                            treatmentDataItem.ClassName = "";
                            if (gbsBackGroundName.Any())
                            {
                                switch (gbsBackGroundName)
                                {
                                    case "TreatmentImage":
                                        var backGroundShapeName = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
                                        if (backGroundShapeName.Any())
                                        {
                                            treatmentDataItem.ClassName = backGroundShapeName.FirstOrDefault().Name;
                                        }
                                        break;
                                    case "TreatmentFill":
                                        var backGroundFillOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
                                        if (backGroundFillOption.Any())
                                        {
                                            var fillOptionValue = backGroundFillOption.FirstOrDefault().Name;
                                            switch (fillOptionValue)
                                            {
                                                case "TreatmentFillPattern":
                                                    var img = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
                                                    if (img.Any())
                                                    {
                                                        treatmentDataItem.DefaultColor = "background-image:url('" + img.FirstOrDefault().ColorSquaresRgb + "')";

                                                    }
                                                    break;
                                                case "TreatmentFillColor":
                                                    var color = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
                                                    if (color.Any())
                                                    {
                                                        treatmentDataItem.DefaultColor = "background-color:" + color.FirstOrDefault().ColorSquaresRgb;

                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }

                        }
                    }
                    else
                    {
                        var defaultColorOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "DefaultEnvelopeColor");
                        var defaultEnvelopType = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Orientation");
                        if (defaultEnvelopType.Any())
                        {
                            string className = defaultEnvelopType.FirstOrDefault().Name;
                            treatmentDataItem.ClassName = className;

                            if (defaultColorOption.Any())
                            {
                                var optionValue = defaultColorOption.FirstOrDefault().ColorSquaresRgb;
                                treatmentDataItem.DefaultColor = optionValue;
                                if (optionValue.Contains("#") && optionValue.Length == 7)
                                {
                                    treatmentDataItem.DefaultColor = "background-color:" + optionValue;
                                }
                                else
                                {
                                    treatmentDataItem.DefaultColor = "background-image:url('" + optionValue + "')";
                                }
                            }
                            else
                            {
                                treatmentDataItem.DefaultColor = "";
                            }
                        }
                    }
                }
                treatmentDataList.Add(treatmentDataItem);
                
            }
            return treatmentDataList;
        }

        public  List<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> getLegacyOrders()
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

            var myOrders = new List<CustomerOrderListModel.OrderDetailsModel>();

            //DataView orderItemResult = manager.GetParameterizedDataView(select, paramDicEx);
            //if (orderItemResult.Count > 0)
            //{
            //     myOrders = JsonConvert.DeserializeObject<List<CustomerOrderListModel.OrderDetailsModel>>((string)orderItemResult[0][0]);

            //}
            string jsonResult = manager.GetParameterizedJsonString(select, paramDicEx);
            myOrders = myOrders = JsonConvert.DeserializeObject<List<CustomerOrderListModel.OrderDetailsModel>>(jsonResult);
            return myOrders;
        }
    }
}
