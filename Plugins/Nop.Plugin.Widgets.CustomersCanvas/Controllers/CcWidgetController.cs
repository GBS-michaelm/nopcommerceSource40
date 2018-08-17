using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Serialization;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Controllers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers
{
    public class CcWidgetController : BasePluginController
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICcService _ccService;
        private readonly IShoppingCartService _cartService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly CcSettings _customersCanvasSettings;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ShoppingCartController _shoppingCartController;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region ctor
        public CcWidgetController(IProductService productService,
            IOrderService orderService,
            ICcService ccService,
            IShoppingCartService cartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductAttributeService productAttributeService,
            ISettingService settingService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IDownloadService downloadService,
            ILocalizationService localizationService)
        {
            _productService = productService;
            _orderService = orderService;
            _ccService = ccService;
            _cartService = cartService;
            _workContext = workContext;
            _storeContext = storeContext;
            _productAttributeService = productAttributeService;
            _settingService = settingService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _customersCanvasSettings = settingService.LoadSetting<CcSettings>();
            _webHelper = webHelper;
            _specificationAttributeService = specificationAttributeService;
            _shoppingCartController = EngineContext.Current.Resolve<ShoppingCartController>();
            _downloadService = downloadService;
            _localizationService = localizationService;
        }
        #endregion

        #region Submit \ update item
        [HttpPost]
        public ActionResult UpdateItem(int productId, int designId, int updateCartItemId, string dataJson, string imagesJson, string optionsJson, 
            string downloadUrlsJson, int quantity, string formOptions = "")
        {
            var product = _productService.GetProductById(productId);
            var customer = _workContext.CurrentCustomer;

            var options = DeserializeOptions(optionsJson);
            var attributesXml = GetProductAttributes(product, dataJson, options, designId, formOptions);

            _ccService.UpdateDesign(designId, dataJson, imagesJson, downloadUrlsJson);

            ShoppingCartItem updatecartitem = null;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            updatecartitem = cart.FirstOrDefault(x => x.Id == updateCartItemId);

            decimal customerEnteredPriceConverted = decimal.Zero;
            _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                updatecartitem.Id, attributesXml, customerEnteredPriceConverted, null, null, quantity);

            //var orderItem = _cartService.FindShoppingCartItemInTheCart(cart, ShoppingCartType.ShoppingCart, product, attributesXml);

            return Json(new { status = "success" }); //itemId = orderItem.Id, designId = designId 
        }

        [HttpPost]
        public ActionResult SubmitItem(int productId, string dataJson, string imagesJson, string downloadUrlsJson, int quantity, string optionsJson = "", string formOptions = "")
        {
            var product = _productService.GetProductById(productId);
            var customer = _workContext.CurrentCustomer;

            var designId = _ccService.AddDesign(dataJson, imagesJson, downloadUrlsJson);

            var options = DeserializeOptions(optionsJson);

            var attributesXml = GetProductAttributes(product, dataJson, options, designId, formOptions);

            var warnings = _cartService.AddToCart(customer, product, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, attributesXml, quantity: quantity);

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //var createdItem = _cartService.FindShoppingCartItemInTheCart(cart, ShoppingCartType.ShoppingCart, product, attributesXml);

            return new StatusCodeResult((int)HttpStatusCode.OK);
            return Json(new { status = "success" }); //itemId = createdItem.Id, designId = designId 
        }
        #endregion

        #region editor page
        public ActionResult EditorPage(int productId, int quantity = 1, int updateCartItemId = 0)
        {
            return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/EditorPage.cshtml",
                new EditorPageModel
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UpdateCartItemId = updateCartItemId
                });
        }

        #endregion

        #region attribute helpers

        private string GetFormOptionsAttributesXml(Product product, string formOptions)
        {
            var nameValueCollection = new Dictionary<string, StringValues>();
            var jsonFormOptions = JObject.Parse(formOptions);
            if (jsonFormOptions != null && jsonFormOptions.Root != null)
            {
                var root = jsonFormOptions.Root;
                if (root.First != null)
                {
                    var item = root.First;
                    while (item != null)
                    {
                        var name = item.Path;
                        var value = item.First.Value<string>();
                        nameValueCollection.Add(name, value);
                        item = item.Next;
                    }
                    var formCollection = new FormCollection(nameValueCollection);
                    var attributesXml = ParseProductAttributes(product, formCollection, new List<string>());
                    attributesXml = attributesXml.Replace("<Attributes>", "").Replace("</Attributes>", "");
                    return attributesXml;
                }
            }
            return "";
        }

        // TODO
        // use it from ShoppingCartController
        private string ParseProductAttributes(Product product, IFormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;

            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[$"product_attribute_{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            return attributesXml;
        }

        private Dictionary<string, SelectedOption> DeserializeOptions(string optionsJson)
        {
            var result = new Dictionary<string, SelectedOption>();
            if (!string.IsNullOrEmpty(optionsJson))
            {
                var converter = new OptionValueConverter();
                result = JsonConvert.DeserializeObject<Dictionary<string, SelectedOption>>(optionsJson, converter);
            }
            return result;
        }

        private string GetProductAttributes(Core.Domain.Catalog.Product product, string dataJson, Dictionary<string, SelectedOption> options, int designId, string formOptions)
        {
            var ccSettings = _settingService.LoadSetting<CcSettings>();
            var productMappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            var productAttributeMappingCcId = productMappings.First(x => x.ProductAttributeId == ccSettings.CcIdAttributeId);

            var optionsAttributesXml = GetOptionsAttributes(product, options);
            var formOptionsAttributeXml = GetFormOptionsAttributesXml(product, formOptions);
            optionsAttributesXml += formOptionsAttributeXml;

            return string.Format("<Attributes>" +
                                 "<ProductAttribute ID=\"{0}\"><ProductAttributeValue><Value>{1}</Value></ProductAttributeValue></ProductAttribute>" +
                                 optionsAttributesXml +
                                 "</Attributes>", productAttributeMappingCcId.Id, designId);
        }

        private string GetOptionsAttributes(Core.Domain.Catalog.Product product, Dictionary<string, SelectedOption> options)
        {
            var result = new StringBuilder();
            foreach (var option in options)
            {
                var val = option.Value;
                var attrId = val.Option.Id;

                if (val.Value != null && val.Value.Any())
                {
                    result.AppendFormat("<ProductAttribute ID=\"{0}\">", attrId);
                    foreach (var selectedOption in val.Value)
                    {
                        result.AppendFormat("<ProductAttributeValue><Value>{0}</Value></ProductAttributeValue>", selectedOption);
                    }
                    result.AppendLine("</ProductAttribute>");
                }
            }
            return result.ToString();
        }
        #endregion


        #region methods
       

   

        public JsonResult GetCartItemsData()
        {
            int count;
            var model = _ccService.GetCcCartItems(out count);

            return new JsonResult(new { Items = model.Items, Count = count });
        }
        #endregion
    }
}
