using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.PriceCalculation.GBS
{
    public class MyPriceCalculationServicePlugin : BasePlugin, IMiscPlugin
    {
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PriceCalculationConfigurationGBS";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.PriceCalculation.GBS.Controllers" },
                { "area", null }
            };
        }
    }
}
