using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Infrastructure
{
    public class LegacyOrderItem : Nop.Core.Domain.Orders.OrderItem
    {
        public string legacyPicturePath { get; set; }
        public string productOptionsJson { get; set; }
        public string webToPrintType { get; set; }
    }

    public class CcService : ICcService
    {
        private readonly IProductService _productService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;
        private readonly IWidgetPlugin _widgetPlugin;
        private readonly WidgetSettings _widgetSettings;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;

        public CcService(IProductService productService,
            IPluginFinder pluginFinder,
            IStoreContext storeContext,
            IWidgetPlugin widgetPlugin,
            WidgetSettings widgetSettings,
            IOrderService orderService,
            ISettingService settingService) {

            _productService = productService;
            _pluginFinder = pluginFinder;
            _storeContext = storeContext;
            _widgetPlugin = widgetPlugin;
            _widgetSettings = widgetSettings;
            _orderService = orderService;
            _settingService = settingService;
        }

        public bool IsPluginEnabled() {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Widgets.CustomersCanvas");
            if (pluginDescriptor == null)
                return false;

            var allowedId = pluginDescriptor.LimitedToStores;
            var pluginAllowed = true;
            if (allowedId.Count > 0)
            {
                if (allowedId.IndexOf(_storeContext.CurrentStore.Id) < 0)
                    pluginAllowed = false;
            }

            return pluginDescriptor.Installed && pluginAllowed &&
                   ((IWidgetPlugin)(pluginDescriptor.Instance())).IsWidgetActive(_widgetSettings);
        }

        public bool IsProductForCc(Product product) {
            var pluginEnabled = IsPluginEnabled();
            if (pluginEnabled)
            {
                var ccIdAttrIdSet = _settingService.GetSetting("ccsettings.ccidattributeid");
                var ccIdAttrId = 0;
                if (ccIdAttrIdSet != null)
                {
                    ccIdAttrId = Convert.ToInt32(ccIdAttrIdSet.Value);
                }
                return _pluginFinder.GetPluginDescriptorBySystemName("Widgets.CustomersCanvas") != null &&
                       product.ProductAttributeMappings.Any(m => m.ProductAttributeId == ccIdAttrId);
            }
            return false;
        }

        public bool IsProductForCc(int productId) {
            var product = _productService.GetProductById(productId);
            return IsProductForCc(product);
        }

        public OrderItem GetOrderItemById(int orderItemId, bool useBase) {
            if (useBase)
            {
                return _orderService.GetOrderItemById(orderItemId);
            }
            return GetOrderItemById(orderItemId);
        }

        public OrderItem GetOrderItemById(int orderItemId) {
            var miscPlugins = _pluginFinder.GetPluginDescriptorBySystemName("Order.GBS");
            if (miscPlugins!= null)
            {
                var orderItem = _orderService.GetOrderItemById(orderItemId);
                if (orderItem == null)
                {
                    orderItem = GetOrderItemById_Legacy(orderItemId, true);
                }
                return orderItem;
            }
            else
            {
                return _orderService.GetOrderItemById(orderItemId);
            }
        }

        public OrderItem GetOrderItemById_Legacy(int orderItemId, bool isLegacy) {
            if (!isLegacy)
            {
                OrderItem orderItem = _orderService.GetOrderItemById(orderItemId);
                return orderItem;
            }
            else
            {
                var connectionStringSet = _settingService.GetSetting("gbsordersettings.homconnectionstring");
                var connectionString = "";
                if (connectionStringSet != null)
                {
                    connectionString = connectionStringSet.Value;
                }

                DBManager manager = new DBManager(connectionString);

                Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                paramDicEx.Add("@orderItemId", orderItemId);

                string select = "EXEC usp_getLegacyOrderItemForNOP @orderItemId";
                //DataView orderResult = manager.GetParameterizedDataView(select, paramDicEx);
                string jsonResult = manager.GetParameterizedJsonString(select, paramDicEx);
                var legacyOrderItem = JsonConvert.DeserializeObject<LegacyOrderItem>(jsonResult);

                return prepareLegacyOrderItem(legacyOrderItem);
            }
        }

        public OrderItem prepareLegacyOrderItem(OrderItem orderItem) {
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
            //_logger.Information("OrderExtensoins prepareLegacyOrderItem orderItem = " + JsonConvert.SerializeObject(orderItem, new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
            //}));

            return orderItem;
        }
    }
}
