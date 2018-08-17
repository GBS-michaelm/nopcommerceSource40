using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using System.Linq;
using System;
using Nop.Services.Configuration;
using Nop.Services.Catalog;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Net;
using Nop.Web.Models.Catalog;
using Nop.Services.Localization;
using Nop.Web.Factories;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "ProductPageEditorInit")]
    public class ProductPageEditorInitViewComponent : NopViewComponent
    {
        private readonly IProductService _productService;
        private readonly ICcService _ccService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly CcSettings _customersCanvasSettings;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IStoreContext _storeContext;

        public ProductPageEditorInitViewComponent(
            IProductService productService,
            ICcService ccService,
            IWorkContext workContext,
            ISettingService settingService,
            ISpecificationAttributeService specificationAttributeService,
            IProductModelFactory productModelFactory,
            IStoreContext storeContext)
        {
            _productService = productService;
            _ccService = ccService;
            _workContext = workContext;
            _settingService = settingService;
            _customersCanvasSettings = settingService.LoadSetting<CcSettings>();
            _specificationAttributeService = specificationAttributeService;
            _productModelFactory = productModelFactory;
            _storeContext = storeContext;
        }

        public IViewComponentResult Invoke(EditorPageModel editorPageModel)
        {
            var result = ProductDetailsAfterBreadcrumb(editorPageModel);
            if (result != null)
            {
                return result;
            }
            return Content("");
        }

        public IViewComponentResult ProductDetailsAfterBreadcrumb(EditorPageModel editorPageModel)
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
                var settings = _settingService.LoadSetting<CcSettings>();
                settings.CustomersCanvasApiKey = "";
                settings.OrderExportPath = "";
                settings.DesignFileName = "";
                var model = new CcEditorLoaderModel
                {
                    PluginSettings = settings,
                    ProductDetails = GetProductDetails(id, updateCartItemId),
                    ProductId = id,
                    Editor = editorJsonData.ToJson(),
                    Config = editorJsonData.GetConfigByTitle(configName).ToJson(),
                    Path = editorJsonData.VirtualPath,
                    EditorFolder = editorJsonData.FolderName,
                    UpdateCartItemId = updateCartItemId,
                    CcData = null,
                    Quantity = quantity,
                    Language = _workContext.WorkingLanguage.LanguageCulture.Split('-')[0],
                    Currency = new CurrencyModel() { Code = _workContext.WorkingCurrency.CurrencyCode, Locale = _workContext.WorkingLanguage.LanguageCulture }
                };
                model.ProductDetails.PageShareCode = "";

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

                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/EditorScripts.cshtml", model);
            }

            return null;
        }

        private ProductDetailsModel GetProductDetails(int productId, int updateCartItemId = 0)
        {
            var controllerContext = new ControllerContext();
            var product = _productService.GetProductById(productId);
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();
            var updatecartitem = cart.FirstOrDefault(x => x.Id == updateCartItemId);
            var model = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);

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
                                    WebUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                                break;
                            case SpecificationAttributeType.CustomText:
                                m.ValueRaw = WebUtility.HtmlEncode(psa.CustomValue);
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
    }
}
