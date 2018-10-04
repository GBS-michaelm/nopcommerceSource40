using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.GBS.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            ViewEngines.Engines.Insert(0, new PluginViewEngine());
           // ViewEngines.Engines.Add(new PluginViewEngine());

            var route = routes.MapRoute("ProductExtendedFields",
            "Admin/category/ProductExtendedFields/{productId}",
            new { controller = "ExtendedFields", action = "ProductExtendedFields" },
            new[] { "Nop.Plugin.Shipping.GBS.Controllers" }
            );

            route.DataTokens.Add("area", "admin");
            routes.Remove(route);
            routes.Insert(0, route);
        }

        public int Priority => -2;
    }
}