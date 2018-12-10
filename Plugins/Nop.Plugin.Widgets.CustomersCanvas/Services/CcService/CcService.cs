using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Plugin.Widgets.CustomersCanvas.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;

namespace Nop.Plugin.Widgets.CustomersCanvas.Services
{
    public class CcService : ICcService
    {
        #region fields

        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly WidgetSettings _widgetSettings;
        private readonly ILogger _logger;
        private readonly CcSettings _customersCanvasSettings;
        private readonly IRepository<CcDesign> _ccDesignRepository;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        #endregion

        #region ctor
        public CcService(ISettingService settingService,
            IPluginFinder pluginFinder,
            WidgetSettings widgetSettings,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            ILogger logger,
            IRepository<CcDesign> ccDesignRepository,
            IProductService productService,
            IStoreContext storeContext,
            IOrderService orderService,
            IWorkContext workContext)
        {
            _settingService = settingService;
            _pluginFinder = pluginFinder;
            _widgetSettings = widgetSettings;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _logger = logger;
            _customersCanvasSettings = settingService.LoadSetting<CcSettings>();
            _ccDesignRepository = ccDesignRepository;
            _productService = productService;
            _storeContext = storeContext;
            _orderService = orderService;
            _workContext = workContext;
        }
        #endregion

        #region cc-design
        public CcDesign GetDesign(int id)
        {
            return _ccDesignRepository.GetById(id);
        }

        public void UpdateDesign(int designId, string data, string imageUrl, string downloadUrlsJson)
        {
            var ccDesign = _ccDesignRepository.GetById(designId);
            ccDesign.Data = data;
            ccDesign.ImageUrl = imageUrl;
            ccDesign.DownloadUrlsJson = downloadUrlsJson;
            _ccDesignRepository.Update(ccDesign);
        }
        public int AddDesign(string data, string imagesUrl, string downloadUrlsJson)
        {
            var ccDesign = new CcDesign();
            ccDesign.Data = data;
            ccDesign.ImageUrl = imagesUrl;
            ccDesign.DownloadUrlsJson = downloadUrlsJson;
            _ccDesignRepository.Insert(ccDesign);

            return ccDesign.Id;
        }

        #endregion

        #region attributes

        public Dictionary<CcProductAttributes, string> CCAttributesSettingsNames()
        {
            return new Dictionary<CcProductAttributes, string>
            {
                {CcProductAttributes.CcId, "ccsettings.ccidattributeid" }
            };
        }

        public IEnumerable<int> GetCcAttrIds(IEnumerable<CcProductAttributes> attrs = null)
        {
            if (attrs == null)
                attrs = Enum.GetValues(typeof(CcProductAttributes)).Cast<CcProductAttributes>();
            return
                attrs.Select(attr => CCAttributesSettingsNames()[attr])
                    .Select(attrSettingName => Convert.ToInt32(_settingService.GetSetting(attrSettingName).Value));
        }

        public int GetCcAttrId(CcProductAttributes attribute)
        {
            return Convert.ToInt32(_settingService.GetSetting(CCAttributesSettingsNames()[attribute]).Value);
        }

        public string ServerHostUrl()
        {
            return _customersCanvasSettings.ServerHostUrl;
        }

        public int EditorDefinitionSpecificationAttributeOptionId()
        {
            return _customersCanvasSettings.EditorDefinitionSpecificationOptionId;
        }

        public int EditorDefinitionSpecificationAttributeId()
        {
            return _customersCanvasSettings.EditorDefinitionSpecificationAttributeId;
        }

        public int EditorConfigurationSpecificationAttributeId()
        {
            return _customersCanvasSettings.EditorConfigurationSpecificationAttributeId;
        }

        public int EditorConfigurationSpecificationAttributeOptionId()
        {
            return _customersCanvasSettings.EditorConfigurationSpecificationOptionId;
        }

        public bool IsItCcHiddenAttributeInProductPage(int attributeId)
        {
            return GetCcAttrIds().Contains(attributeId);
        }

        public bool IsProductForCc(Product product)
        {
            var pluginEnabled = IsPluginEnabled();
            if (pluginEnabled)
            {
                var ccIdAttrId = _customersCanvasSettings.CcIdAttributeId;
                return _pluginFinder.GetPluginDescriptorBySystemName("Widgets.CustomersCanvas") != null &&
                       product.ProductAttributeMappings.Any(m => m.ProductAttributeId == ccIdAttrId);
            }
            return false;
        }

        public bool IsProductForCc(int productId)
        {
            var product = _productService.GetProductById(productId);
            return IsProductForCc(product);
        }

        #endregion

        #region scripts
        public string JsWritePromisePolyfill()
        {
            var promiseScripts = new[]
            {
                "~/Plugins/Widgets.CustomersCanvas/Scripts/es6-promise/es6-promise.min.js",
                "~/Plugins/Widgets.CustomersCanvas/Scripts/es6-promise/auto-polyfill-es6-promise.js"
            };
            return JsWriteScriptTags(promiseScripts); //.Select(VirtualPathUtility.ToAbsolute)
        }

        public string JsWriteScriptTags(IEnumerable<string> scripts)
        {
            var writeCode = new StringBuilder();

            foreach (var scriptUrl in scripts)
            {
                writeCode.Append("document.write('");
                writeCode.Append(JavaScriptEncoder.Default.Encode(
                    "<script src=\"" + scriptUrl + "\" type=\"text/javascript\"></script>"));
                writeCode.Append("');\n");
            }
            return writeCode.ToString();
        }
        #endregion

        public bool IsPluginEnabled()
        {
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

        public string RemoveAttrMappings(string attributesXml, IEnumerable<int> productAttributesIds)
        {
            var xmlDoc = new XmlDocument();
            if (string.IsNullOrEmpty(attributesXml))
                return attributesXml;

            xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
            if (rootElement == null)
                return attributesXml;

            var nodeList = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
            if (nodeList == null)
                return attributesXml;

            Func<XmlNode, ProductAttributeMapping> getProductAttributeMapping = n =>
            {
                if (n.Attributes == null)
                    return null;

                var idStr = n.Attributes["ID"].InnerText.Trim();

                int mappingId;
                if (!int.TryParse(idStr, out mappingId))
                    return null;

                return _productAttributeService.GetProductAttributeMappingById(mappingId);
            };

            var nodeToRemove = nodeList.Cast<XmlNode>().Where(n =>
            {
                var mapping = getProductAttributeMapping(n);

                return mapping != null && productAttributesIds.Contains(mapping.ProductAttributeId);
            }).ToList();

            foreach (var node in nodeToRemove)
                rootElement.RemoveChild(node);

            return xmlDoc.OuterXml;
        }

        #region cc Result

        public CcResult GetCcResult(string attributesXml)
        {
            var mappings = _productAttributeParser.ParseProductAttributeMappings(attributesXml);
            var mapping = mappings.FirstOrDefault(x => x.ProductAttributeId == _customersCanvasSettings.CcIdAttributeId);
            if (mapping == null)
                return null;

            var values = _productAttributeParser.ParseValues(attributesXml, mapping.Id);
            if (values == null || !values.Any())
                return null;

            var designId = Convert.ToInt32(values.First());

            var design = this.GetDesign(designId);

            string[] hiresUrls = null;
            if (!string.IsNullOrEmpty(design.DownloadUrlsJson))
            {
                hiresUrls = JsonConvert.DeserializeObject<string[]>(design.DownloadUrlsJson);
            }
            string[] proofImages = new[] { design.ImageUrl };

            try // для того, чтобы старые значения не ломались
            {
                if (!string.IsNullOrEmpty(design.ImageUrl))
                {
                    proofImages = JsonConvert.DeserializeObject<string[]>(design.ImageUrl);
                }
            }
            catch (Exception ex) { }

            var ccResult = new CcResult()
            {
                ProofUrls = proofImages,
                HiResUrls = hiresUrls
            };

            return ccResult;
        }

        #endregion

        #region cc-widget

        public string GetEditorDefinition(int productId)
        {
            var product = _productService.GetProductById(productId);
            return GetEditorDefinition(product);
        }

        public string GetEditorDefinition(Product product)
        {
            var productSpecificationAttribute = product.ProductSpecificationAttributes.FirstOrDefault(
                spec =>
                    spec.SpecificationAttributeOption.SpecificationAttributeId ==
                    _customersCanvasSettings.EditorDefinitionSpecificationAttributeId);

            if (productSpecificationAttribute == null)
                return "{}";  //throw new ArgumentException("This product isn't CC product");

            var productSpecificationStr = productSpecificationAttribute.CustomValue;

            return productSpecificationStr;
        }

        public string GetEditorConfiguration(int productId)
        {
            var product = _productService.GetProductById(productId);
            return GetEditorConfiguration(product);
        }
        public string GetEditorConfiguration(Product product)
        {
            var editorConfigurationAttribute = product.ProductSpecificationAttributes.FirstOrDefault(
                spec =>
                    spec.SpecificationAttributeOption.SpecificationAttributeId ==
                    _customersCanvasSettings.EditorConfigurationSpecificationAttributeId);

            if (editorConfigurationAttribute == null)
                return "{}";  //throw new ArgumentException("This product isn't CC product");
            var editorConfiguratironStr = editorConfigurationAttribute.CustomValue;

            return editorConfiguratironStr;
        }

        #endregion

        #region editors

        public List<EditorData> GetAllEditors()
        {
            var path = CommonHelper.MapPath(Path.Combine(PluginPaths.PluginFolder, PluginPaths.EditorsFolder)).Replace("\\~\\","\\");
            Directory.CreateDirectory(path);
            var editorFolders = Directory.EnumerateDirectories(path).ToList();
            var editors = new List<EditorData>();
            foreach (var editorFolder in editorFolders)
            {
                if (EditorData.IsValidEditorFolder(editorFolder))
                {
                    editors.Add(new EditorData(editorFolder));
                }

            }
            return editors;
        }

        public EditorData GetEditor(string title)
        {
            var editor = GetAllEditors().FirstOrDefault(x => x.Title == title);
            if (editor != null)
            {
                return editor;
            }

            _logger.InsertLog(LogLevel.Error, string.Format("GetEditor: Editor file not found. Title: {0}", title));
            throw new Exception("Editor file not found");
        }

        public ConfigData GetConfig(string editorName, string configName)
        {
            var editor = GetAllEditors().FirstOrDefault(x => x.Title == editorName);
            if (editor != null)
            {
                var config = editor.GetConfigByTitle(configName);
                if (config != null)
                {
                    return config;
                }

                _logger.InsertLog(LogLevel.Error, string.Format("GetConfig: Config file not found. EditorName: {0} configName: {1}", editorName, configName));
                throw new Exception("Config file not found");
            }

            _logger.InsertLog(LogLevel.Error, string.Format("GetConfig:  Editor file not found. EditorName: {0} configName: {1}", editorName, configName));
            throw new Exception("Editor file not found");
        }

        public List<string> GetCcOptionsFromConfig(int productId)
        {
            List<string> ccAttributeList = new List<string>();

            var product = _productService.GetProductById(productId);
            var editorName = GetEditorDefinition(product);
            var configName = GetEditorConfiguration(product);

            var editorJsonData = GetEditor(editorName);
            var config = editorJsonData.GetConfigByTitle(configName).ToJson();
            dynamic cfg = JsonConvert.DeserializeObject(config);
            try
            {
                if (cfg.config["optionNames"] != null)
                {
                    var optionNames = cfg.config["optionNames"];
                    foreach (dynamic option in optionNames)
                    {
                        ccAttributeList.Add(option.Value.name.Value);
                    }
                }
                if (cfg.config["steps"] != null)
                {
                    var steps = cfg.config["steps"];
                    if (steps["editor"] != null)
                    {
                        var editor = steps["editor"];
                        if (editor["optionNames"] != null)
                        {
                            var optionNames = editor["optionNames"];
                            foreach (dynamic option in optionNames)
                            {
                                ccAttributeList.Add(option.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }

            ccAttributeList.Add("CcId");

            return ccAttributeList;
        }
        #endregion
        public string FormatOrderTokenString(Core.Domain.Orders.OrderItem item, string str)
        {
            var order = _orderService.GetOrderById(item.OrderId);
            var product = _productService.GetProductById(item.ProductId);
            var index = order.OrderItems.ToList().IndexOf(item);
            var result = str
               .Replace("<order_id>", order.Id.ToString())
               .Replace("<order_guid>", order.OrderGuid.ToString())
               .Replace("<store_id>", order.StoreId.ToString())
               .Replace("<customer_id>", order.CustomerId.ToString())
               .Replace("<affiliate_id>", order.AffiliateId.ToString())
               .Replace("<product_id>", product.Id.ToString())
               .Replace("<product_name>", product.Name ?? "")
               .Replace("<product_sku>", product.Sku ?? "")
               .Replace("<product_vendorId>", product.VendorId.ToString())
               .Replace("<item_guid>", item.OrderItemGuid.ToString())
               .Replace("<item_index>", index.ToString())
               .Replace("<item_quantity>", item.Quantity.ToString());

            foreach (var spec in product.ProductSpecificationAttributes)
            {
                var specName = spec.SpecificationAttributeOption.Name;
                var specValue = spec.CustomValue;
                result = result.Replace("<specattr_" + specName + ">", specValue);
            }
            result = result.Replace(" ", "");
            return result;
        }


        public CcCartImagesReplacementModel GetCcCartItems(out int count)
        {
            var model = new CcCartImagesReplacementModel();
            var ccSettings = _settingService.LoadSetting<CcSettings>();

            var customer = _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            int cartItemIndex = 0;
            foreach (var shoppingCartItem in cart)
            {
                var attributeMappings = _productAttributeParser.ParseProductAttributeMappings(shoppingCartItem.AttributesXml);
                foreach (var attributeMapping in attributeMappings)
                {
                    if (attributeMapping.ProductAttributeId == ccSettings.CcIdAttributeId)
                    {
                        var values = _productAttributeParser.ParseValues(shoppingCartItem.AttributesXml, attributeMapping.Id);
                        if (values != null && values.Any())
                        {
                            var value = Convert.ToInt32(values.First());
                            var design = GetDesign(value);
                            var imageUrl = design.ImageUrl;

                            try   // чтобы не падало на старых значениях
                            {
                                imageUrl = JsonConvert.DeserializeObject<string[]>(design.ImageUrl).First();
                            }
                            catch (Exception ex) { }

                            model.Items.Add(new CcCartImagesReplacementModel.Item()
                            {
                                CartItemId = shoppingCartItem.Id,
                                Index = cartItemIndex,
                                ImageSource = imageUrl,
                            });
                        }
                    }
                }
                cartItemIndex++;
            }
            count = cartItemIndex;
            return model;
        }
    }
}
