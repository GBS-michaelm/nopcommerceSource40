using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Localization;
using System.Collections.Generic;

namespace Nop.Plugin.Catalog.GBS
{
    public class CategoryNavigationProvider : BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        public CategoryNavigationProvider(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "category_navigation_block" };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsCategoryNavigation/Configure";
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <param name="viewComponentName">View component name</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
        {
            viewComponentName = "CategoryNavigation";
        }

        /// <summary>
        /// Install plugin
        /// </summary>  
        public override void Install()
        {              
            //locales
            //this.AddOrUpdatePluginLocaleResource("admin.catalog.GBS.Enable", "Is Enable");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Catalog.GBS.AllCategory", "Bring All Categories");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Catalog.GBS.NoOfChildren", "No of Levels");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Catalog.GBS.IsActive", "Is Active");
                   
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {           
            //locales            
            //this.DeletePluginLocaleResource("admin.catalog.GBS.Enable");
            this.DeletePluginLocaleResource("Nop.Plugin.Catalog.GBS.AllCategory");
            this.DeletePluginLocaleResource("Nop.Plugin.Catalog.GBS.NoOfChildren");
            this.DeletePluginLocaleResource("Nop.Plugin.Catalog.GBS.IsActive");

            base.Uninstall();
        }       
    }
}
