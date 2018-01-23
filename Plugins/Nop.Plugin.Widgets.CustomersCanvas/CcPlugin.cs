using Nop.Core.Domain.Catalog;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Plugin.Widgets.CustomersCanvas.Data;

namespace Nop.Plugin.Widgets.CustomersCanvas
{
    public class CcPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {

        #region fields
        private readonly ISettingService _settingService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly CcDesignObjectContext _ccDesignObjectContext;
        #endregion

        #region ctor
        public CcPlugin(ISettingService settingService,
            IProductAttributeService productAttributeService,
            ISpecificationAttributeService specificationAttributeService,
            IProductTemplateService productTemplateService,
            IProductService productService,
            IUrlRecordService urlRecordService,
            CcDesignObjectContext ccDesignObjectContext)
        {
            _settingService = settingService;
            _productAttributeService = productAttributeService;
            _specificationAttributeService = specificationAttributeService;
            _productTemplateService = productTemplateService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _ccDesignObjectContext = ccDesignObjectContext;
        }
        #endregion

        public override void Install()
        {


            var settings = new CcSettings
            {
                ServerHostUrl = @"",

            };
            InstallAttributes(settings);


            _settingService.SaveSetting(settings);

            #region language resources

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportDownloadError",
                "Download error. See System -> Log for details.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportFinish",
                "PDF files are successfully exported.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportEmpty",
                "No designs in the selected orders!");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportPathWarning",
                "Error! Set existing path where exported PDF files should be saved in the Aurigma Personalization Plugin settings!");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportButton",
                "Get PDF designs from orders");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.DesignButton",
                "Personalize");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditor.SelectFromList",
                "Choose editor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.SelectFromList",
                "Choose config");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributes",
                "Clear editor and config");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributesConfirm",
                "Are you sure you want to delete editor and config attributes for this product?");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFonts",
                "Reload Customer's Canvas fonts");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFontsButton",
                "Reload");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.ServerHostUrl.Hint",
                "URL of your Customer's Canvas instance");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.ServerHostUrl",
                "Customer's Canvas URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.config.DesignFileName",
                "File name template the exported PDF files (without extension)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.CustomersCanvasApiKeyWarning",
                "Set the CustomersCanvasApiSecurityKey value in Nopcommerce web.config appSettings. It should be the same as specified in your Customer's Canvas instance.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.IsOrderExportButton",
                "Add 'Get PDF designs from orders' button to Admin Order List view");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.OrderExportPath",
               "Path to the folder for the exported PDF files");

            #endregion

            _ccDesignObjectContext.Install();
            base.Install();
        }

        public override void Uninstall()
        {
            var settings = _settingService.LoadSetting<CcSettings>();

            _settingService.DeleteSetting<CcSettings>();

            #region language resources
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportDownloadError");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportFinish");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportEmpty");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.OrderExportPathWarning");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.DesignButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditor.SelectFromList");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.SelectFromList");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributes");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributesConfirm");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.CustomersCanvasApiKeyWarning");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.DesignFileName");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFonts");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFontsButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.ServerHostUrl");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.ServerHostUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.IsOrderExportButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.OrderExportPath");

            #endregion

            base.Uninstall();
        }

        private void InstallAttributes(CcSettings settings)
        {
            var attributes = _productAttributeService.GetAllProductAttributes();
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes();

            var ccIdAttribute = attributes.FirstOrDefault(x => x.Name == "CcId");
            if (ccIdAttribute == null)
            {
                ccIdAttribute = new ProductAttribute() { Name = "CcId" };
                _productAttributeService.InsertProductAttribute(ccIdAttribute);
            }

            var editorDefinitionSpecificationAttribute = specificationAttributes.FirstOrDefault(x => x.Name == "CC Editor");
            if (editorDefinitionSpecificationAttribute == null)
            {
                editorDefinitionSpecificationAttribute = new SpecificationAttribute
                {
                    Name = "CC Editor",
                    DisplayOrder = 100
                };
                _specificationAttributeService.InsertSpecificationAttribute(editorDefinitionSpecificationAttribute);
            }

            var editorDefinitionSpecificationOption = editorDefinitionSpecificationAttribute.SpecificationAttributeOptions.FirstOrDefault(x => x.Name == "CC Editor");
            if (editorDefinitionSpecificationOption == null)
            {
                editorDefinitionSpecificationOption = new SpecificationAttributeOption
                {
                    DisplayOrder = 0,
                    Name = "CC Editor",
                    SpecificationAttributeId = editorDefinitionSpecificationAttribute.Id
                };
                _specificationAttributeService.InsertSpecificationAttributeOption(editorDefinitionSpecificationOption);
            }

            var editorConfigurationSpecificationAttribute = specificationAttributes.FirstOrDefault(x => x.Name == "CC EditorConfig");
            if (editorConfigurationSpecificationAttribute == null)
            {
                editorConfigurationSpecificationAttribute = new SpecificationAttribute
                {
                    Name = "CC EditorConfig",
                    DisplayOrder = 100
                };
                _specificationAttributeService.InsertSpecificationAttribute(editorConfigurationSpecificationAttribute);
            }

            var editorConfigurationSpecificationOption = editorDefinitionSpecificationAttribute.SpecificationAttributeOptions.FirstOrDefault(x => x.Name == "CC EditorConfig");
            if (editorConfigurationSpecificationOption == null)
            {
                editorConfigurationSpecificationOption = new SpecificationAttributeOption
                {
                    DisplayOrder = 0,
                    Name = "CC EditorConfig",
                    SpecificationAttributeId = editorConfigurationSpecificationAttribute.Id
                };
                _specificationAttributeService.InsertSpecificationAttributeOption(editorConfigurationSpecificationOption);
            }

            // add to settings
            settings.CcIdAttributeId = ccIdAttribute.Id;
            settings.EditorDefinitionSpecificationAttributeId = editorDefinitionSpecificationAttribute.Id;
            settings.EditorDefinitionSpecificationOptionId = editorDefinitionSpecificationOption.Id;
            settings.EditorConfigurationSpecificationAttributeId = editorConfigurationSpecificationAttribute.Id;
            settings.EditorConfigurationSpecificationOptionId = editorConfigurationSpecificationOption.Id;
        }

        private void UninstallAttributes(CcSettings settings)
        {
            // remove product attributes
            var attribute = _productAttributeService.GetProductAttributeById(settings.CcIdAttributeId);
            if (attribute != null)
                _productAttributeService.DeleteProductAttribute(attribute);

            // remove specification attributes
            var specificationAttribute =
                 _specificationAttributeService.GetSpecificationAttributeById(settings.EditorConfigurationSpecificationAttributeId);
            if (specificationAttribute != null)
                _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            specificationAttribute =
                 _specificationAttributeService.GetSpecificationAttributeById(settings.EditorDefinitionSpecificationAttributeId);
            if (specificationAttribute != null)
                _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            var specificationAttributeOption =
                 _specificationAttributeService.GetSpecificationAttributeOptionById(settings.EditorDefinitionSpecificationOptionId);
            if (specificationAttributeOption != null)
                _specificationAttributeService.DeleteSpecificationAttributeOption(specificationAttributeOption);

            specificationAttributeOption =
                 _specificationAttributeService.GetSpecificationAttributeOptionById(settings.EditorConfigurationSpecificationOptionId);
            if (specificationAttributeOption != null)
                _specificationAttributeService.DeleteSpecificationAttributeOption(specificationAttributeOption);
        }

        #region create/delete example data
        private void CreateExampleProducts(CcSettings settings)
        {
            var product = new Product
            {
                ProductType = ProductType.SimpleProduct,
                ProductTemplateId = _productTemplateService.GetAllProductTemplates().First().Id,
                Name = "Business Card",
                VisibleIndividually = true,
                ShowOnHomePage = true,
                AllowCustomerReviews = true,
                Price = 10,
                Published = true,
                CreatedOnUtc = DateTime.Now,
                UpdatedOnUtc = DateTime.Now,
                AvailableStartDateTimeUtc = new DateTime(2015, 6, 1),
                OrderMaximumQuantity = 100,
                OrderMinimumQuantity = 1
            };

            _productService.InsertProduct(product);
            if (settings.SampleProducts == null)
                settings.SampleProducts = new List<int>();
            settings.SampleProducts.Add(product.Id);

            var productDefinition = new ProductSpecificationAttribute
            {
                AllowFiltering = true,
                AttributeType = SpecificationAttributeType.CustomText,
                SpecificationAttributeOptionId = settings.EditorDefinitionSpecificationOptionId,
                CustomValue = @"Default editor",
                DisplayOrder = 0,
                ProductId = product.Id,
                ShowOnProductPage = false
            };

            _specificationAttributeService.InsertProductSpecificationAttribute(productDefinition);

            var editorConfiguration = new ProductSpecificationAttribute
            {
                AllowFiltering = true,
                AttributeType = SpecificationAttributeType.CustomText,
                SpecificationAttributeOptionId = settings.EditorConfigurationSpecificationOptionId,
                CustomValue = @"Default config",
                DisplayOrder = 0,
                ProductId = product.Id,
                ShowOnProductPage = false
            };
            _specificationAttributeService.InsertProductSpecificationAttribute(editorConfiguration);

            #region product attribute mappings
            _productAttributeService.InsertProductAttributeMapping(new ProductAttributeMapping
            {
                AttributeControlType = AttributeControlType.TextBox,
                DefaultValue = "",
                DisplayOrder = 100,
                ProductId = product.Id,
                ProductAttributeId = settings.CcIdAttributeId
            });
            #endregion

            _urlRecordService.SaveSlug(product, product.ValidateSeName(product.Name, product.Name, true), 0);

        }

        private void DeleteExampleProducts(CcSettings settings)
        {
            if (settings.SampleProducts != null)
            {
                foreach (var productId in settings.SampleProducts)
                {
                    var product = _productService.GetProductById(productId);
                    if (product != null)
                        _productService.DeleteProduct(product);
                }
            }
        }
        #endregion

        #region IWidgetPlugin
        public void GetConfigurationRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "CcConfig";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.CustomersCanvas.Controllers"},
                {"area", null}
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "CustomersCanvasWidget";
            controllerName = "CcWidget";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.CustomersCanvas.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                "orderdetails_product_line",  // на странице просмотра ордера добавляет информацию из СС
                "productdetails_add_info", // на вьюшке AddToCart - заменяем кнопку AddToCart
                "productdetails_bottom",    // на странице продукта после всего
                "order_summary_content_after",  // на странице корзины заменяем всем СС продуктам картинки
                "head_html_tag",        // head документа. Добавляет web-components и пр.
                "product_editor_page"       // на странице СС редактора продукта
            };
        }
        #endregion

        #region IAdminMenuPlugin
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            // todo
            //var menuItem = new SiteMapNode()
            //{
            //    Title = "Personalization Plugin Editors",
            //    SystemName = "CustomersCanvas",
            //    ControllerName = "CcEditor",
            //    ActionName = "Index",
            //    Visible = true,
            //    RouteValues = new RouteValueDictionary() { { "area", null } },
            //};

            //// если сделать группу и подгруппу
            ////var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            ////if (pluginNode != null)
            ////    pluginNode.ChildNodes.Add(menuItem);
            ////else
            //rootNode.ChildNodes.Add(menuItem);
        }
        #endregion
    }
}
