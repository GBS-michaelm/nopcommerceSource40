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

            // todo remove excess resources
            #region language resources

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CustomersCanvasApiKeyWarning",
                "Please, set the CustomersCanvasApiSecurityKey value in Nopcommerce web.config appSettings");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.DesignButton",
                "Customize Card");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditor.SelectFromList",
               "Aurigma Editor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.SelectFromList",
               "Editor's Config");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributes",
              "Remove Editor and Config Attributes");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributesConfirm",
             "Delete Editor and Config Attributes for this product?");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFonts",
             "Reload CustomersCanvas fonts");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFontsButton",
             "Reload");
           

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ServerHostUrl.Hint",
                "URL of Customer`s Canvas instance");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ServerHostUrl",
                "Server host URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CreateDesignPageTitle",
                "Create your design");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.FinishDesign",
                "Order product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.DesignNow",
                "Design now");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SaveProject",
                "Save project");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SetProjectNameWindowLabel",
                "Project name:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.UniqueProjectNameWarning",
                "Project with the name <PROJECT NAME> exists.\n" +
                "Click OK if you want to overwrite the existing project.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.UserNotRegistered",
                "User not registered");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SetProjectNameWindowTitle",
                "Please name your project");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SetProjectNameWindowButton",
                "OK");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ReviewYourBook",
                "Review Your Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Approval",
                "Approval");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ApprovalText",
                "The product will be printed exactly as it appears above. By clicking the \"Approve\" button below, I agree that spelling, content and layout are correct.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ApproveButton",
                "Approve →");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.MakeSomeChangesButton",
                "← I want to make some changes");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.BootDesigner",
                "Product designer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.Enabled",
                "Enabled:");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetDesignFrom",
                "Set product template from:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetDesignFrom.IndividualFiles",
                "individual design files");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetDesignFrom.Folder",
                "folder with design files");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.DesignFolderName",
                "Product template folder name:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.DesignFilePage1",
                "Design template name for page1:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.DesignFilePage2",
                "Design template name for page2:");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetMockupFrom",
                "Set mockup template from:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetMockupFrom.IndividualFiles",
                "individual mockup files");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetMockupFrom.Folder",
                "folder with mockup files");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupFolderName",
                "Mockup template folder name:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupFile1",
                "Mockup template name for page1:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupFile2",
                "Mockup template name for page2:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupType",
                "Set mockup type:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupType.Front",
               "Foreground mockup");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupType.Down",
               "Background mockup");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.Bleed",
                "Bleed area mm.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.Margin",
                "Margin area mm.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.AdvancedModeDisabled",
                "Simple mode only:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.EditorConfiguration",
                "EditorConfiguration:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.ProductDefinition",
                "ProductDefinition:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.ShowAdvancedOptions",
                "Show advanced options");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.HideAdvancedOptions",
                "Hide advanced options");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ErrorMessage",
                "We’re sorry, a system error occurred. We apologize for the interruption to your work. Please contact our customer service team for assistance on info@example.com ");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ContinueWithoutSaving", "Continue without saving");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SaveBeforeEdit", "You need to save the product before you can add Customer's Canvas settings");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Order.UploadImages", "Upload user images");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Order.DownloadImages.All", "Download user images (all found)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Order.DownloadImages.Selected", "Download user images (selected)");

            #endregion

            _ccDesignObjectContext.Install();
            base.Install();
        }

        public override void Uninstall()
        {
            var settings = _settingService.LoadSetting<CcSettings>();

            _settingService.DeleteSetting<CcSettings>();

            #region language resources

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.DesignButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditor.SelectFromList");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.SelectFromList");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributes");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CcEditorConfig.RemoveEditorConfigAttributesConfirm");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFonts");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Config.UpdateFontsButton");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ServerHostUrl");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ServerHostUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.CreateDesignPageTitle");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.FinishDesign");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.DesignNow");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SaveProject");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SetProjectNameWindowLabel");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SetProjectNameWindowTitle");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SetProjectNameWindowButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.UniqueProjectNameWarning");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ReviewYourBook");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Approval");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ApprovalText");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ApproveButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.MakeSomeChangesButton");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.BootDesigner");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.Enabled");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetDesignFrom");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetDesignFrom.IndividualFiles");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetDesignFrom.Folder");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.DesignFolderName");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.DesignFilePage1");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.DesignFilePage2");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetMockupFrom");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetMockupFrom.IndividualFiles");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.SetMockupFrom.Folder");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupFolderName");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupFile1");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupFile2");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupType.Front");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupType.Down");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.MockupType");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.Bleed");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.Margin");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.AdvancedModeDisabled");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.EditorConfiguration");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.ProductDefinition");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.ShowAdvancedOptions");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ProductEdit.HideAdvancedOptions");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ErrorMessage");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.ContinueWithoutSaving");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.SaveBeforeEdit");

            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Order.UploadImages");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Order.DownloadImages.All");
            this.DeletePluginLocaleResource("Plugins.Widgets.CustomersCanvas.Order.DownloadImages.Selected");
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
            var menuItem = new SiteMapNode()
            {
                SystemName = "CustomersCanvas",
                Title = "CustomersCanvas Editors",
                ControllerName = "CcEditor",
                ActionName = "Index",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            // если сделать группу и подгруппу
            //var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            //if (pluginNode != null)
            //    pluginNode.ChildNodes.Add(menuItem);
            //else
                rootNode.ChildNodes.Add(menuItem);
        }
        #endregion
    }
}
