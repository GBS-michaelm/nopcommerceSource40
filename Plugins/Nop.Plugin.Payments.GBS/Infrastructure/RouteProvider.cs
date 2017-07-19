using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.GBS.Infrastructure
{
    public partial class GBSPaymentRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
           
            routes.MapLocalizedRoute("CustomerPaymentMethods",
                            "customer/paymentmethods",
                            new { controller = "PaymentProfile", action = "PaymentMethods" },
                            new[] { "Nop.Plugin.Payments.GBS.Controllers" });
            routes.MapLocalizedRoute("CustomerPaymentMethodEdit",
                            "customer/paymentmethodedit/{profileId}",
                            new { controller = "PaymentProfile", action = "PaymentMethodEdit" },
                            new { profileId = @"\d+" },
                            new[] { "Nop.Plugin.Payments.GBS.Controllers" });
            routes.MapLocalizedRoute("CustomerPaymentMethodAdd",
                            "customer/paymentmethodadd",
                            new { controller = "PaymentProfile", action = "PaymentMethodAdd" },
                            new[] { "Nop.Plugin.Payments.GBS.Controllers" });
            routes.MapLocalizedRoute("CustomerPaymentMethodDelete",
                            "customer/paymentmethoddelete",
                            new { controller = "PaymentProfile", action = "PaymentMethodDelete" },
                            new[] { "Nop.Plugin.Payments.GBS.Controllers" });

            ViewEngines.Engines.Insert(0, new GBSPaymentViewEngine());

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
