using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Order.GBS.Infrastructure
{
    public partial class GBSOrderRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
           
            routes.MapRoute("CanvasUpdateProductView",
                            "order/updateproduct",
                            new { controller = "GBSOrder", action = "UpdateCanvasProductView" });

            routes.MapRoute("CopyFilesToProduction",
                            "order/CopyFilesToProduction",
                            new { controller = "GBSOrder", action = "CopyFilesToProduction" });

            routes.MapRoute("OrderDetailsL",
                            "orderdetailsL/{orderId}",
                            new { controller = "GBSOrder", action = "DetailsLegacy" }, new { orderId = @"\d+" });

            routes.MapRoute("GetOrderPdfInvoiceL",
                            "orderdetailsL/pdf/{orderId}",
                            new { controller = "GBSOrder", action = "GetPdfInvoiceLegacy" });

             routes.MapRoute("GBSCustomerOrders",
                            "order/history",
                            new { controller = "CustomerOrder", action = "CustomerOrders" });
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
