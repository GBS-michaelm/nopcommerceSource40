using Nop.Core.Domain.Payments;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Web.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Serialization;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Controllers;
using Newtonsoft.Json.Linq;

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
        private readonly ProductController _productController;
        private readonly ShoppingCartController _shoppingCartController;
        private readonly IDownloadService _downloadService;
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
            IDownloadService downloadService)
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
            _productController = EngineContext.Current.Resolve<ProductController>();
            _shoppingCartController = EngineContext.Current.Resolve<ShoppingCartController>();
            _downloadService = downloadService;
        }
        #endregion

        #region Submit \ update item
        [HttpPost]
        [ValidateInput(false)]
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
        [ValidateInput(false)]
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

       //     var createdItem = _cartService.FindShoppingCartItemInTheCart(cart, ShoppingCartType.ShoppingCart, product, attributesXml);
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

        #region helpers

        private string GetFormOptionsAttributesXml(Product product, string formOptions)
        {
            var nameValueCollection = new System.Collections.Specialized.NameValueCollection();
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
                    var attributesXml = ParseProductAttributes(product, formCollection);
                    attributesXml = attributesXml.Replace("<Attributes>", "").Replace("</Attributes>", "");
                    return attributesXml;
                }
            }
            return "";
        }

        // TODO
        // use it from ShoppingCartController
        private string ParseProductAttributes(Product product, FormCollection form)
        {
            string attributesXml = "";

            #region Product attributes

            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        var ctrlAttributes = form[controlId];
                        if (!String.IsNullOrEmpty(ctrlAttributes))
                        {
                            int selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }
                        break;
                    case AttributeControlType.Checkboxes:
                    {
                        var ctrlAttributes = form[controlId];
                        if (!String.IsNullOrEmpty(ctrlAttributes))
                        {
                            foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                int selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
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
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }
                    }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        var ctrlAttributes = form[controlId];
                        if (!String.IsNullOrEmpty(ctrlAttributes))
                        {
                            string enteredText = ctrlAttributes.Trim();
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
                        Guid downloadGuid;
                        Guid.TryParse(form[controlId], out downloadGuid);
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

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

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

        #region widget
        [ChildActionOnly]
        public ActionResult CustomersCanvasWidget(string widgetZone, object additionalData)
        {
            switch (widgetZone)
            {
                case "orderdetails_product_line":
                    if (additionalData != null && additionalData is int)
                    {
                        return OrderItemDetailsCcResult((int)additionalData);
                    }
                    break;
                case "productdetails_bottom":
                    if (additionalData != null && additionalData is int)
                    {
                        return HideAttributes((int)additionalData);
                    }
                    break;
                case "productdetails_add_info":
                    var data = 0;
                    if (additionalData != null && additionalData is int)
                    {
                        data = (int)additionalData;
                    }
                    else
                    {
                        var reqData = ControllerContext.HttpContext.Request.RequestContext.RouteData.Values["productid"];
                        if (reqData != null)
                        {
                            data = (int)reqData;
                        }
                    }
                    if (data != 0)
                    {
                        return ChangeAddToCartButton(data);
                    }
                    break;
                case "order_summary_content_after":
                    return OrderSummaryContentAfter();
                case "head_html_tag":
                    return HeadHtmlTag();
                case "product_editor_page":
                    return ProductEditorPageWidget();
                default:
                    return new EmptyResult();
            }
            return new EmptyResult();
        }

        #endregion

        #region methods

        private ActionResult HideAttributes(int id)
        {
            var isProductForCc = _ccService.IsProductForCc(id);
            if (isProductForCc)
            {
                var product = _productService.GetProductById(id);
                var mapping = product.ProductAttributeMappings;
                var ccOptionNames = _ccService.GetCcOptionsFromConfig(id);
                var optionsIdList = new List<int>();
                foreach (var optionName in ccOptionNames)
                {
                    var option = mapping.FirstOrDefault(x => x.ProductAttribute.Name == optionName);
                    if (option != null)
                    {
                        optionsIdList.Add(option.Id);
                    }
                }
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/_HideAttributes.cshtml",
                    optionsIdList);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult ChangeAddToCartButton(int id)
        {
            var isProductForCc = _ccService.IsProductForCc(id);
            if (isProductForCc)
            {
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/_ChangeAddToCartButton.cshtml", id);
            }
            else
            {
                return new EmptyResult();
            }
        }

        private ActionResult ProductEditorPageWidget()
        {
            return View("~/Plugins/Widgets.CustomersCanvas/Views/ProductEditorPageWidget.cshtml");
        }

        private ActionResult HeadHtmlTag()
        {
            return View("~/Plugins/Widgets.CustomersCanvas/Views/_Head.cshtml");
        }

        public PartialViewResult ProductDetailsAfterBreadcrumb(EditorPageModel editorPageModel)
        {
            var productId = editorPageModel.ProductId;
            var quantity = editorPageModel.Quantity;
            var updateCartItemId = editorPageModel.UpdateCartItemId;

            var id = productId;
            var product = _productService.GetProductById(id);
            if (product != null && _ccService.IsProductForCc(product))
            {
                var editorName = _ccService.GetEditorDefinition(product);
                var configName = _ccService.GetEditorConfiguration(product);

                var editorJsonData = _ccService.GetEditor(editorName);
                var model = new CcEditorLoaderModel
                {
                    PluginSettings = _settingService.LoadSetting<CcSettings>(),
                    ProductDetails = GetProductDetails(id, updateCartItemId),
                    ProductId = id,
                    Editor = editorJsonData.ToJson(),
                    Config = editorJsonData.GetConfigByTitle(configName).ToJson(),
                    Path = editorJsonData.VirtualPath,
                    EditorFolder = editorJsonData.FolderName,
                    UpdateCartItemId = updateCartItemId,
                    CcData = null,
                    Quantity = quantity,
                    Language = _workContext.WorkingLanguage.LanguageCulture.Split('-')[0]
                };

                var userModel = new UserModel();
                userModel.UserGuid = _workContext.CurrentCustomer.CustomerGuid.ToString().Replace("-", "");
                userModel.UserId = _workContext.CurrentCustomer.Id;
                userModel.IsAuthenticated = User.Identity.IsAuthenticated;
                model.User = userModel;

                if (updateCartItemId != 0)
                {
                    var ccIdAttribute = model.ProductDetails.ProductAttributes.FirstOrDefault(x => x.ProductAttributeId == _customersCanvasSettings.CcIdAttributeId);
                    int designId = 0;
                    if (ccIdAttribute != null)
                    {
                        if (Int32.TryParse(ccIdAttribute.DefaultValue, out designId))
                        {
                            model.CcData = _ccService.GetDesign(designId);
                        }
                    }
                }

                return PartialView("~/Plugins/Widgets.CustomersCanvas/Views/Product/EditorScripts.cshtml", model);
            }

            return null;
        }

        private ActionResult OrderSummaryContentAfter()
        {
            int count;
            var list = new List<ICcCartScriptItem>();

            var imageModel = GetCcCartItems(out count);
            var returnToEditModel = GetCcCartReturnToEditItems();
            list.AddRange(imageModel.Items);
            list.AddRange(returnToEditModel.Items);
            return View("~/Plugins/Widgets.CustomersCanvas/Views/ShoppingCart/_CartScripts.cshtml", list);
        }


        private CcCartReturnToEditReplacementModel GetCcCartReturnToEditItems()
        {
            var model = new CcCartReturnToEditReplacementModel();
            var ccSettings = _settingService.LoadSetting<CcSettings>();

            var customer = _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            foreach (var shoppingCartItem in cart)
            {
                var attributeMappings =
                    _productAttributeParser.ParseProductAttributeMappings(shoppingCartItem.AttributesXml);
                foreach (var attributeMapping in attributeMappings)
                {
                    if (attributeMapping.ProductAttributeId == ccSettings.CcIdAttributeId)
                    {
                        var editCartItemUrl = Url.Action("EditorPage", "CcWidget", new { productId = shoppingCartItem.Product.Id });
                        editCartItemUrl += "&quantity=" + shoppingCartItem.Quantity + "&updateCartItemId=" + shoppingCartItem.Id;

                        var oldUrl = Url.RouteUrl("Product", new { SeName = shoppingCartItem.Product.GetSeName() });
                        oldUrl = _webHelper.ModifyQueryString(oldUrl, "updatecartitemid=" + shoppingCartItem.Id, null);

                        model.Items.Add(new CcCartReturnToEditReplacementModel.Item()
                        {
                            CartItemId = shoppingCartItem.Id,
                            OldUrl = oldUrl,
                            Url = editCartItemUrl
                        });

                    }
                }
            }
            return model;
        }

        private CcCartImagesReplacementModel GetCcCartItems(out int count)
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
                            var design = _ccService.GetDesign(value);
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

        private ProductDetailsModel GetProductDetails(int productId, int updateCartItemId = 0)
        {
            _productController.Url = new UrlHelper(new RequestContext(ControllerContext.HttpContext, new RouteData()));
            var productDetails = _productController.ProductDetails(productId, updateCartItemId);

            ViewResult viewResult = (ViewResult)productDetails;
            var model = (ProductDetailsModel)viewResult.ViewData.Model;

            // add hidden specification params
            // which checked as "don't show on product page"
            model.ProductSpecifications =
                _specificationAttributeService.GetProductSpecificationAttributes(productId, 0, null, null)
                    .Select(psa =>
                    {
                        var m = new ProductSpecificationModel
                        {
                            SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                            SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute
                                .GetLocalized(x => x.Name),
                            ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb
                        };

                        switch (psa.AttributeType)
                        {
                            case SpecificationAttributeType.Option:
                                m.ValueRaw =
                                    HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                                break;
                            case SpecificationAttributeType.CustomText:
                                m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
                                break;
                            case SpecificationAttributeType.CustomHtmlText:
                                m.ValueRaw = psa.CustomValue;
                                break;
                            case SpecificationAttributeType.Hyperlink:
                                m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
                                break;
                            default:
                                break;
                        }
                        return m;
                    })
                    .ToList();

            return model;
        }



        [NonAction]
        private ActionResult OrderItemDetailsCcResult(int id)
        {
            var item = _orderService.GetOrderItemById(id);
            var ccResultData = _ccService.GetCcResult(item.AttributesXml);
            if (ccResultData != null)
            {
                if (item.Order.PaymentStatus != PaymentStatus.Paid || item.Order.OrderStatus != OrderStatus.Complete)
                {
                    ccResultData.HiResUrls = null;
                }
                ccResultData.ReturnToEditUrl = null;
                ccResultData.HiResOutputName = item.Product.Name;
            }
            return PartialView("~/Plugins/Widgets.CustomersCanvas/Views/Order/_CcResult.cshtml", ccResultData);
        }

        public JsonResult GetCartItemsData()
        {
            int count;
            var model = GetCcCartItems(out count);

            return Json(new { Items = model.Items, Count = count }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
