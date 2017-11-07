using System;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Infrastructure;
using Nop.Web.Framework.Localization;


namespace Nop.Plugin.BusinessLogic.GBS.Infrastructure
{
    public partial class BusinessLogicRouteProvider : IRouteProvider
    {
               

        public void RegisterRoutes(RouteCollection routes)
        {
            
            routes.MapLocalizedRoute("GenerateTeamHtml",
                           "sportsgateway/SportsTeamHtml/{id}",
                           new { controller = "GatewayPageController", action = "SportsTeamHtml" },
                           new[] { "Nop.Plugin.GBSGateway.GBS.Controllers" });

            routes.MapLocalizedRoute("GatewayCatalogProducts",
                            "sportsgateway/GatewayCatalogProducts/{type}/{id}",
                            new { controller = "GatewayPage", action = "GatewayCatalogProducts" },
                            new[] { "Nop.Plugin.GBSGateway.GBS.Controllers" });

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
