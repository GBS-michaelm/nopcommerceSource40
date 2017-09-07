using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Order.GBS.Infrastructure
{
    public partial class GBSOrderRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
           
            routes.MapLocalizedRoute("CanvasUpdateProductView",
                            "order/updateproduct",
                            new { controller = "GBSOrder", action = "UpdateCanvasProductView" },
                            new[] { "Nop.Plugin.Order.GBS.Controllers" });
            routes.MapLocalizedRoute("CopyFilesToProduction",
                            "order/CopyFilesToProduction",
                            new { controller = "GBSOrder", action = "CopyFilesToProduction" },
                            new[] { "Nop.Plugin.Order.GBS.Controllers" });
        }

        public int Priority
        {
            get
            {
                return 2000;
            }
        }
    }
}
