using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web;
using System;
using System.Web.Routing;

namespace Nop.Plugin.Order.GBS
{
    public class MyOrderServicePlugin : BasePlugin, IMiscPlugin
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
    }
}
