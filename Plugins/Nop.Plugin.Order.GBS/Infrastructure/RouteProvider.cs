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
                            new { controller = "Order", action = "UpdateCanvasProductView" },
                            new[] { "Nop.Plugin.Order.GBS.Controllers" });
            routes.MapLocalizedRoute("CopyFilesToProduction",
                            "order/CopyFilesToProduction",
                            new { controller = "Order", action = "CopyFilesToProduction" },
                            new[] { "Nop.Plugin.Order.GBS.Controllers" });
            routes.MapLocalizedRoute("OrderDetailsL",
                            "orderdetailsL/{orderId}",
                            new { controller = "Order", action = "DetailsLegacy" },
                            new { orderId = @"\d+" },
                            new[] { "Nop.Plugin.Order.GBS.Controllers" });
            routes.MapLocalizedRoute("GetOrderPdfInvoiceL",
                            "orderdetailsL/pdf/{orderId}",
                            new { controller = "Order", action = "GetPdfInvoiceLegacy" },
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
