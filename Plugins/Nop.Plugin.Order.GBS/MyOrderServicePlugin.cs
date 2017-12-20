using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web;
using System;
using System.Web.Routing;
using Nop.Services.Cms;
using System.Collections.Generic;

namespace Nop.Plugin.Order.GBS
{
    public class MyOrderServicePlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ConfigurationGBS";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Order.GBS.Controllers" },
                { "area", null }
            };
            //throw new NotImplementedException();
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
            if (widgetZone == "order_addPhoneNumber_widget") 
            {
                    actionName = "AddPhoneNumber";
                    controllerName = "GBSOrder";
                    routeValues = new RouteValueDictionary()
                    {
                        { "Namespaces", "Nop.Plugin.Order.GBS.Controllers" },
                        { "area", null }
                    };
            } else 
            {
                actionName = "DisplayLegacyOrderItemImage";
                controllerName = "GBSOrder";
                routeValues = new RouteValueDictionary()
                    {
                        { "Namespaces", "Nop.Plugin.Order.GBS.Controllers" },
                        { "area", "" }
                    };
            }
        }
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "order_addPhoneNumber_widget", "orderdetails_product_line","orderdetails_product_line_legacy" }; 
        }
    }
}
