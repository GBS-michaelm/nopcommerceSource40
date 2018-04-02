using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Catalog.GBS
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("SportsTeamsList",
                            "SportsTeamsList",
                            new { controller = "GBSCatalog", action = "SportsTeamsList" },
                            new[] { "Nop.Plugin.Catalog.GBS.Controllers" });

            var adminListroute = routes.MapRoute("List",
            "Admin/Product/List",
            new { controller = "GBSProductAdmin", action = "List" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );

            adminListroute.DataTokens.Add("area", "admin");
            routes.Remove(adminListroute);
            routes.Insert(0, adminListroute);
        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }
    }
}
