using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.Marketing.Infrastructure
{
    public partial class MarketingRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
           
            routes.MapLocalizedRoute("MarketingWidgetConfigure",
                            "marketing/configure",
                            new { controller = "WidgetsMarketing", action = "Configure" },
                            new[] { "Nop.Plugin.Widgets.Marketing.Controllers" });

            routes.MapLocalizedRoute("MarketingWidgetEmailPreferences",
                            "marketing/emailpreferences",
                            new { controller = "EmailPref", action = "EmailPreferencesView" },
                            new[] { "Nop.Plugin.Widgets.Marketing.Controllers" });

            ViewEngines.Engines.Insert(0, new MarketingViewEngine());

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
