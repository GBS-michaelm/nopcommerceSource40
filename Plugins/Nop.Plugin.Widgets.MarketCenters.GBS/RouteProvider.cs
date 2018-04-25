using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.MarketCenters.GBS
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.MarketCenters",
                 "Plugin/MarketCenters",
                 new { controller = "MarketCenters", action = "Index", },
                 new[] { "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers" }
            );

            routes.MapRoute("Plugin.MarketCenters.CreateClient",
                 "Plugin/MarketCenters/CreateClient",
                 new { controller = "MarketCenters", action = "CreateClient", },
                 new[] { "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers" }
            );

            routes.MapRoute("Plugin.MarketCenters.EditClient",
                 "Plugin/MarketCenters/EditClient",
                 new { controller = "MarketCenters", action = "EditClient" },
                 new[] { "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers" }
            );

                    routes.MapRoute("Plugin.MarketCenters.AssociateSpecificBacks",
             "Plugin/MarketCenters/AssociateSpecificBacks",
             new { controller = "MarketCenters", action = "AssociateSpecificBacks" },
             new[] { "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers" }
        );
                    routes.MapRoute("Plugin.MarketCenters.CustomizeDesign",
             "Plugin/MarketCenters/CustomizeDesign",
             new { controller = "MarketCenters", action = "CustomizeDesign" },
             new[] { "Nop.Plugin.Widgets.MarketCenters.GBS.Controllers" }
        );

        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
