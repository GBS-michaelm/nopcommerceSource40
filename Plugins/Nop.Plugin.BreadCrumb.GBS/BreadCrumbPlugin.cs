using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.BreadCrumb.GBS
{
    public class BreadCrumbPlugin : BasePlugin, IMiscPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "BreadCrumbGBS";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.BreadCrumb.GBS.Controllers" },
                { "area", null }
            };
        }
    }
}
