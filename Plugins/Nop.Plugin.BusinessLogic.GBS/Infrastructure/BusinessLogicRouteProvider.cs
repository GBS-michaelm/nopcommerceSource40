using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.BusinessLogic.GBS.Infrastructure
{
    public partial class BusinessLogicRouteProvider : IRouteProvider
    {       
        public void RegisterRoutes(IRouteBuilder routes)
        {            
            routes.MapRoute("GenerateTeamHtml",
                           "sportsgateway/SportsTeamHtml/{id}",
                           new { controller = "GatewayPageController", action = "SportsTeamHtml" });

            routes.MapRoute("GatewayCatalogProducts",
                            "sportsgateway/GatewayCatalogProducts/{type}/{id}",
                            new { controller = "GatewayPage", action = "GatewayCatalogProducts" });

            routes.MapRoute("MarketCenterTabs",
                            "marketcentergateway/MarketCenterGatewayTabs/{id}/{type}",
                            new { controller = "GatewayPage", action = "MarketCenterTabs" });

            routes.MapRoute("AccessoryPage",
                            "accessory/{groupId}/{productId}",
                            new { controller = "AccessoryPage", action = "AccessoryPage" });

            routes.MapRoute("AccessoryCategories",
                            "accessory/AccessoryCategories/{groupid}",
                            new { controller = "AccessoryPage", action = "AccessoryCategories" });

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
