using Nop.Core.Plugins;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.CanvasOverride.GBS
{
    public class CanvasOverridePlugin:  BasePlugin, IMiscPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            //actionName = "Configure";
            //controllerName = "Checkout";
            //routeValues = new RouteValueDictionary()
            //{
            //    { "Namespaces", "Nop.Plugin.Checkout.GBS.Controllers" },
            //    { "area", null }
            //};
            throw new NotImplementedException();
        }
    }
}
