using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using Nop.Services.Cms;

namespace Nop.Plugin.Catalog.GBS
{
    public class CategoryNavigationProvider : BasePlugin, IWidgetPlugin
    {    
        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "category_navigation_block" };
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
            controllerName = "WidgetsCategoryNavigation";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Catalog.GBS.Controllers" }, { "area", null } };
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
            actionName = "CategoryNavigation";
            controllerName = "WidgetsCategoryNavigation";            
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Catalog.GBS.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}                
            };
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
