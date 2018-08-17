using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.GBS.Infrastructure
{
    public partial class GBSPaymentRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
           
            routes.MapRoute("CustomerPaymentMethods",
                            "customer/paymentmethods",
                            new { controller = "PaymentProfile", action = "PaymentMethods" });

            routes.MapRoute("CustomerPaymentMethodEdit",
                            "customer/paymentmethodedit/{profileId}",
                            new { controller = "PaymentProfile", action = "PaymentMethodEdit" },
                            new { profileId = @"\d+" });

            routes.MapRoute("CustomerPaymentMethodAdd",
                            "customer/paymentmethodadd",
                            new { controller = "PaymentProfile", action = "PaymentMethodAdd" });

            routes.MapRoute("CustomerPaymentMethodDelete",
                            "customer/paymentmethoddelete",
                            new { controller = "PaymentProfile", action = "PaymentMethodDelete" });
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
