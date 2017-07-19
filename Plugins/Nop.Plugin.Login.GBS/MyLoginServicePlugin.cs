using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.Login.GBS
{
    public class MyLoginServicePlugin : BasePlugin, IMiscPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "LoginConfigurationGBS";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Login.GBS.Controllers" },
                { "area", null }
            };
        }
    }
}
