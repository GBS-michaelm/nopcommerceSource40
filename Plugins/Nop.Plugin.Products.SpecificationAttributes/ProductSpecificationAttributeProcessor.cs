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
            return new List<string> { "product_artist_name","product_listing_widget","product_details_widget",
                "productdetails_bottom", "product_category_titles", "product_by_artist",
                "categorydetails_top","productbox_addinfo_after" ,"order_listing_widget","orderdetails_product_line_product"};   /*,"orderdetails_product_line"*/
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

            var Treatment = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Treatment").FirstOrDefault();
            if (Treatment == null)
            {
                Treatment = new Core.Domain.Catalog.SpecificationAttribute();
                Treatment.Name = "Treatment";

                var _specificationOptionsTreatmentImage = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsTreatmentImage.Name = "TreatmentImage";
                Treatment.SpecificationAttributeOptions.Add(_specificationOptionsTreatmentImage);


                var _specificationOptionsTreatmentFill = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsTreatmentFill.Name = "TreatmentFill";
                Treatment.SpecificationAttributeOptions.Add(_specificationOptionsTreatmentFill);


                _specificationAttributeService.InsertSpecificationAttribute(Treatment);
            }
            var TreatmentImage = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "TreatmentImage").FirstOrDefault();
            if (TreatmentImage == null)
            {
                TreatmentImage = new Core.Domain.Catalog.SpecificationAttribute();
                TreatmentImage.Name = "TreatmentImage";
                _specificationAttributeService.InsertSpecificationAttribute(TreatmentImage);
            }

            var TreatmentFill = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "TreatmentFill").FirstOrDefault();
            if (TreatmentFill == null)
            {
                TreatmentFill = new Core.Domain.Catalog.SpecificationAttribute();
                TreatmentFill.Name = "TreatmentFill";

                var _specificationOptionsTreatmentFillColor = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsTreatmentFillColor.Name = "TreatmentFillColor";
                TreatmentFill.SpecificationAttributeOptions.Add(_specificationOptionsTreatmentFillColor);


                var _specificationOptionsTreatmentFillPattern = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsTreatmentFillPattern.Name = "TreatmentFillPattern";
                TreatmentFill.SpecificationAttributeOptions.Add(_specificationOptionsTreatmentFillPattern);

                _specificationAttributeService.InsertSpecificationAttribute(TreatmentFill);

            }
            var TreatmentFillColor = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "TreatmentFillColor").FirstOrDefault();
            if (TreatmentFillColor == null)
            {
                TreatmentFillColor = new Core.Domain.Catalog.SpecificationAttribute();
                TreatmentFillColor.Name = "TreatmentFillColor";
                _specificationAttributeService.InsertSpecificationAttribute(TreatmentFillColor);
            }

            var TreatmentFillPattern = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "TreatmentFillPattern").FirstOrDefault();
            if (TreatmentFillPattern == null)
            {
                TreatmentFillPattern = new Core.Domain.Catalog.SpecificationAttribute();
                TreatmentFillPattern.Name = "TreatmentFillPattern";
                _specificationAttributeService.InsertSpecificationAttribute(TreatmentFillPattern);
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
