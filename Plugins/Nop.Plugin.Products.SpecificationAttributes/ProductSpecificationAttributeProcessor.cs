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
                "categorydetails_top","productbox_addinfo_after" ,"order_listing_widget"};   /*,"orderdetails_product_line"*/
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
            //var DefaultEnvelopeColorAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "DefaultEnvelopeColor").FirstOrDefault();
            //if (DefaultEnvelopeColorAttr == null)
            //{
            //    DefaultEnvelopeColorAttr = new Core.Domain.Catalog.SpecificationAttribute();
            //    DefaultEnvelopeColorAttr.Name = "DefaultEnvelopeColor";
            //    _specificationAttributeService.InsertSpecificationAttribute(DefaultEnvelopeColorAttr);
            //}
            //var OrientationAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Orientation").FirstOrDefault();
            //if (OrientationAttr == null)
            //{
            //    OrientationAttr = new Core.Domain.Catalog.SpecificationAttribute();
            //    OrientationAttr.Name = "Orientation";

            //    var _specificationOptionsVertical = new Core.Domain.Catalog.SpecificationAttributeOption();
            //    _specificationOptionsVertical.Name = "Vertical";
            //    OrientationAttr.SpecificationAttributeOptions.Add(_specificationOptionsVertical);

            //    var _specificationOptionsHorizontal = new Core.Domain.Catalog.SpecificationAttributeOption();
            //    _specificationOptionsHorizontal.Name = "Horizontal";
            //    OrientationAttr.SpecificationAttributeOptions.Add(_specificationOptionsHorizontal);

            //    _specificationAttributeService.InsertSpecificationAttribute(OrientationAttr);
            //}
            var ThumbnailBackground = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Treatment").FirstOrDefault();
            if (ThumbnailBackground == null)
            {
                ThumbnailBackground = new Core.Domain.Catalog.SpecificationAttribute();
                ThumbnailBackground.Name = "Treatment";

                var _specificationOptionsThumbnailBackgroundQS = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsThumbnailBackgroundQS.Name = "TreatmentImage";
                ThumbnailBackground.SpecificationAttributeOptions.Add(_specificationOptionsThumbnailBackgroundQS);


                var _specificationOptionsThumbnailBackgroundE = new Core.Domain.Catalog.SpecificationAttributeOption();
                _specificationOptionsThumbnailBackgroundE.Name = "TreatmentFill";
                ThumbnailBackground.SpecificationAttributeOptions.Add(_specificationOptionsThumbnailBackgroundE);


                _specificationAttributeService.InsertSpecificationAttribute(ThumbnailBackground);
            }
            var GBSBackGroundShape = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "TreatmentImage").FirstOrDefault();
            if (GBSBackGroundShape == null)
            {
                GBSBackGroundShape = new Core.Domain.Catalog.SpecificationAttribute();
                GBSBackGroundShape.Name = "TreatmentImage";
                _specificationAttributeService.InsertSpecificationAttribute(GBSBackGroundShape);
            }
            
            var GBSBackGroundfill = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "TreatmentFill").FirstOrDefault();
            if (GBSBackGroundfill == null)
            {
                GBSBackGroundfill = new Core.Domain.Catalog.SpecificationAttribute();
                GBSBackGroundfill.Name = "TreatmentFill";
                _specificationAttributeService.InsertSpecificationAttribute(GBSBackGroundfill);

                //var _specificationOptionsThumbnailBackgroundQS = new Core.Domain.Catalog.SpecificationAttributeOption();
                //_specificationOptionsThumbnailBackgroundQS.Name = "GBSBackGroundColor";
                //GBSBackGroundfill.SpecificationAttributeOptions.Add(_specificationOptionsThumbnailBackgroundQS);


                //var _specificationOptionsThumbnailBackgroundE = new Core.Domain.Catalog.SpecificationAttributeOption();
                //_specificationOptionsThumbnailBackgroundE.Name = "GBSBackGroundImage";
                //GBSBackGroundfill.SpecificationAttributeOptions.Add(_specificationOptionsThumbnailBackgroundE);


            }
            //var GBSBackGroundColor = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "GBSBackGroundColor").FirstOrDefault();
            //if (GBSBackGroundColor == null)
            //{
            //    GBSBackGroundColor = new Core.Domain.Catalog.SpecificationAttribute();
            //    GBSBackGroundColor.Name = "GBSBackGroundColor";
            //    _specificationAttributeService.InsertSpecificationAttribute(GBSBackGroundColor);
            //}

            //var GBSBackGroundImage = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "GBSBackGroundImage").FirstOrDefault();
            //if (GBSBackGroundImage == null)
            //{
            //    GBSBackGroundImage = new Core.Domain.Catalog.SpecificationAttribute();
            //    GBSBackGroundImage.Name = "GBSBackGroundImage";
            //    _specificationAttributeService.InsertSpecificationAttribute(GBSBackGroundImage);
            //}
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
