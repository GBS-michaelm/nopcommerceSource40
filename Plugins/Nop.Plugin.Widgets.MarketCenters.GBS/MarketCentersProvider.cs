using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Plugin.Widgets.MarketCenters.GBS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Services.Localization;
using Nop.Services.Cms;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Widgets.MarketCenters.GBS
{
    public class MarketCentersProvider : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly MarketCentersObjectContext _objectContext;

        public MarketCentersProvider(MarketCentersObjectContext objectContext)
        {
            this._objectContext = objectContext;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            // define widget zone at here
            return new List<string> { "home_page_top" };
        }
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "MarketCenters";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "MarketCenters";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>  
        public override void Install()
        {
            //database objects
            _objectContext.Install();            

            //locales
            // Menus
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Menu.Title.Clients", "Clients");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Menu.Title.ClientList", "List");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Menu.Title.ClientDetails", "Details");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Menu.Title.Templates", "Templates");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Menu.Title.ProductsSetUp", "Product Set Up");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Menu.Title.MarketCenters", "Market Centers");

            // Tabs
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Tabs.Details", "Details");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Tabs.Contact", "Contact / Login");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Tabs.Style", "Style Guide");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Tabs.Products", "Products");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Tabs.Proofs", "Proofs");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Tabs.Clients", "Clients");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Tabs.Templates", "Templates");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Tabs.Products", "Products");
            //this.AddOrUpdatePluginLocaleResource(".Hint", "");            
            //this.AddOrUpdatePluginLocaleResource(".Required", "required");     
       
            // Client Details
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Title.Manage", "Manage Marketcenter");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.AddNew", "Add Client");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.BackToList", "back to clients");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.EditClientDetails", "Edit Client");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.CompanyTitle", "Company Name");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.RelationshipsTitle", "Relationships");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.BrandingTitle", "Branding");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.AccessTitle", "Access");            
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Id", "Client Id");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Name", "Full Name");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.ShortVariableName", "Short Variable Name");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.ParentCompany", "Parent Company");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.RelatedCompany", "Related Companies");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Visible", "Visible");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.LogoDisplay", "Logo OK To Display");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.AlternateLogoId", "Alternate Display Logo");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.LeadingHeader", "Leading Header");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.LeadingCustomHeader", "Leading Custom Header");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.UrlStub", "URL Stub");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.AuthorizeAPI", "Authorize API");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.RequirePassword", "Require Password");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Parent", "Parent");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.AccountRep", "Account Rep");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proofs", "Proofs");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.InMenu", "In Menu");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.CompanyName", "Company / Office Name");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.SearchStore", "Store");

            // Contacts / Login
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Contacts.AccountContacts", "Account Contact");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Contacts.GBSAccountRep", "GBS Account Rep");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Contact.ContactName", "Contact Name");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Contact.ContactEmail", "Contact Email");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Contact.ContactPhone", "Contact Phone");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Contact.ContactNameGBS", "Contact Name");

            // Style Guide
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Logos", "Logos");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Textures", "Colors and Background Textures");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.EditColor", "Edit Colors");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Fonts", "Fonts");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Symbols", "Symbols");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AdvancedEditor", "Advanced Editor");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.ProductColorFamily", "Product Color Family");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PlaceholderText", "Placeholder Text");            
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimaryHorizontalLight", "Primary horizontal on light");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimaryHorizontalDark", "Primary horizontal on dark");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimarySquareLight", "Primary square on light");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimarySquareDark", "Primary square on dark");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.OneColorBlackHorizontal", "One color black horizontal");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.OneColorBlackVertical", "One color black vertical");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.OneColorWhiteHorizontal", "One color white horizontal");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.OneColorWhiteVertical", "One color white verticle");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AltHorizontalLight", "Alt horizontal on light");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AltHorizontalDark", "Alt horizontal on dark");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AltSquareLight", "Alt on square light");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AltSquareDark", "Alt on square black");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimaryColor", "Primary color");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.SecondaryColor", "Secondary color");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AccentColor", "Accent color");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.LightBodyColor", "Light body color");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.DarkBodyColor", "Dark body color");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.LightBackground", "Text on light background");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.DarkBackground", "Text on dark background");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Background1", "Background 1");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Background2", "Background 2");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Background3", "Background 3");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.ColorPalette", "Restrict Color Palette");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AssignColor", "Assign Color by Product");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimaryFont", "Primary Font");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.PrimaryLetterSpecing", "Primary Letter Specing");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.SecondaryFont", "Secondary Font");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.SecondaryLetterSpecing", "Secondary Letter Specing");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.BodyCopyFont", "Body Copy Font");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.BodyLetterSpecing", "Body Letter Specing");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Symbol1", "Default Symbol 1");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Symbol2", "Default Symbol 2");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Symbol3", "Default Symbol 3");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.EnableAdvacedEditor", "Enable Advaced Editor");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.ForceAdvacedEditor", "Force Advaced Editor");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Pen", "Pens");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.ColorEnvelope", "Color Envelopes");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.AgentName", "Agent Name");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Title", "Title");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.MobilePhone", "Mobile Phone");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.OfficePhone", "Office Phone");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Fax", "Fax");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Email", "Email");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Website", "Website");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Address", "Address");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.Liecense", "Liecense");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.StyleGuide.DisclaimerText", "Disclaimer Text");

            // Approved Products
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Products.BusinessCards", "Business Cards");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Products.DirectionalSigns", "Directional Signs");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Products.CarMagnets", "Car Magnets");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Products.Edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Products.Remove", "Remove");

            // Proofs
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.ProofLink", "Client Proof Link");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.Url", "Proof URL");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.SendEmail", "Send Proof Email");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.BusinessCards", "Business Cards");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.BusinessCardTtle", "Business Card");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.Status", "Proof Status");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.CustomizeDesignLink", "Customize Design");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.AssociateLink", "Associate specific backs");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.AddToMarketCenterLink", "Add to Market Center");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.RemoveProof", "Remove Proof");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.ShowProof", "Show Proof");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.BackToProof", "back to proof");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.Categories", "Categories");
            this.AddOrUpdatePluginLocaleResource("MarketCenters.Client.Proof.BusinessCardCustomBlocks", "Business Card Custom Blocks");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //database objects
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("");
            this.DeletePluginLocaleResource(".Required");
            this.DeletePluginLocaleResource(".Hint");            

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var clientMenu = new SiteMapNode()
            {
                SystemName = "Clients",
                Title = EngineContext.Current.Resolve<ILocalizationService>().GetResource("MarketCenters.Menu.Title.Clients"),
                //Url = "/Plugin/MarketCenters",
                Visible = true,
                IconClass = "fa fa-genderless"
            };
            var clientList = new SiteMapNode()
            {
                SystemName = "ClientList",
                Title = EngineContext.Current.Resolve<ILocalizationService>().GetResource("MarketCenters.Menu.Title.ClientList"),
                Url = "/Plugin/MarketCenters",
                Visible = true,
                IconClass = "fa fa-genderless"
            };
            var clientDetailsMenu = new SiteMapNode()
            {
                SystemName = "ClientDetails",
                Title = EngineContext.Current.Resolve<ILocalizationService>().GetResource("MarketCenters.Menu.Title.ClientDetails"),
                Url = "/Plugin/MarketCenters/CreateClient",
                Visible = true,
                IconClass = "fa-dot-circle-o"
            };
            clientMenu.ChildNodes.Add(clientList);
            clientMenu.ChildNodes.Add(clientDetailsMenu);

            var templateMenu = new SiteMapNode()
            {
                SystemName = "Templates",
                Title = EngineContext.Current.Resolve<ILocalizationService>().GetResource("MarketCenters.Menu.Title.Templates"),                
                Visible = true,
                Url = "/Plugin/MarketCenters",
                IconClass = "fa fa-genderless"
            };            
            var productMenu = new SiteMapNode()
            {
                SystemName = "ProductsSetUp",
                Title = EngineContext.Current.Resolve<ILocalizationService>().GetResource("MarketCenters.Menu.Title.ProductsSetUp"),
                Visible = true,
                Url = "/Plugin/MarketCenters",
                IconClass = "fa fa-genderless"
            };
            var topMenu = new SiteMapNode()
            {
                SystemName = "MarketCenters",
                Title = EngineContext.Current.Resolve<ILocalizationService>().GetResource("MarketCenters.Menu.Title.MarketCenters"),                
                Visible = true,                
                IconClass = "fa-dot-circle-o"
            };
            topMenu.ChildNodes.Add(clientMenu);
            topMenu.ChildNodes.Add(templateMenu);
            topMenu.ChildNodes.Add(productMenu);

            rootNode.ChildNodes.Add(topMenu);
        }
    }
}
