using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Localization;
using Nop.Services.Cms;
using System;
using System.Collections.Generic;
using Nop.Services.Catalog;
using System.Linq;

namespace Nop.Plugin.Products.SpecificationAttributes
{
    /// <summary>
    /// Manual payment processor
    /// </summary>
    public class ProductSpecificationAttributeProcessor : BasePlugin, IWidgetPlugin
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;

        public ProductSpecificationAttributeProcessor(ISpecificationAttributeService specificationAttributeService)
        {
            _specificationAttributeService = specificationAttributeService;
        }

        #region Methods

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SpecificationAttributes";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Products.SpecificationAttributes.Controllers" }, { "area", null } };
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
            if (widgetZone == "categorydetails_top")
            {
                actionName = "CategoryImage";
                controllerName = "SpecificationAttributes";
                routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Products.SpecificationAttributes.Controllers" }, { "area", null } };
            }
            else
            {
                actionName = "PublicInfo";
                controllerName = "SpecificationAttributes";
                routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Products.SpecificationAttributes.Controllers" }, { "area", null } };
            }
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { "product_artist_name",
                "productdetails_bottom", "product_category_titles", "product_by_artist",
                "categorydetails_top","productbox_addinfo_after" };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var artistAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Artist").FirstOrDefault();
            if (artistAttr == null)
            {
                artistAttr = new Core.Domain.Catalog.SpecificationAttribute();
                artistAttr.Name = "Artist";
                _specificationAttributeService.InsertSpecificationAttribute(artistAttr);
            }
            var DefaultEnvelopeColorAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "DefaultEnvelopeColor").FirstOrDefault();
            if (DefaultEnvelopeColorAttr == null)
            {
                DefaultEnvelopeColorAttr = new Core.Domain.Catalog.SpecificationAttribute();
                DefaultEnvelopeColorAttr.Name = "DefaultEnvelopeColor";


                _specificationAttributeService.InsertSpecificationAttribute(DefaultEnvelopeColorAttr);
            }
            var OrientationAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Orientation").FirstOrDefault();
            if (OrientationAttr == null)
            {
                OrientationAttr = new Core.Domain.Catalog.SpecificationAttribute();
                OrientationAttr.Name = "Orientation";

                var _specificationOptionsVertical = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsVertical.Name = "Vertical";
                OrientationAttr.SpecificationAttributeOptions.Add(_specificationOptionsVertical);

                var _specificationOptionsHorizontal = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsHorizontal.Name = "Horizontal";
                OrientationAttr.SpecificationAttributeOptions.Add(_specificationOptionsHorizontal);

                _specificationAttributeService.InsertSpecificationAttribute(OrientationAttr);
            }

            this.AddOrUpdatePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.ArtistName", "Artist");
            this.AddOrUpdatePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.ArtistName.Hint", "Artist");
            this.AddOrUpdatePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.Artist", "Artist");
            this.AddOrUpdatePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.Artist.Saved", "Artist Saved");
            this.AddOrUpdatePluginLocaleResource("artistname.by", "by");
            this.AddOrUpdatePluginLocaleResource("Product.Category", "Categories");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.ArtistName");
            this.DeletePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.ArtistName.Hint");
            this.DeletePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.Artist");
            this.DeletePluginLocaleResource("Plugin.Prodcuts.SpecificationAttributes.Artist.Saved");
            this.DeletePluginLocaleResource("artistname.by");
            this.DeletePluginLocaleResource("Product.Category");
            base.Uninstall();
        }

        #endregion        
    }
}
