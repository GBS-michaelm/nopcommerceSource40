using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Catalog.GBS
{
	public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute("SportsTeamsList",
                            "SportsTeamsList",
                            new { controller = "GBSCatalog", action = "SportsTeamsList" });

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
