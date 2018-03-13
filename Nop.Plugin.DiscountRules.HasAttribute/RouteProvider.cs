using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.HasAttribute
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.HasAttribute.Configure",
                 "Plugins/DiscountRulesHasAttribute/Configure",
                 new { controller = "DiscountRulesHasAttribute", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.HasAttribute.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.HasAttribute.AttributeAddPopup",
                 "Plugins/DiscountRulesHasAttribute/AttributeAddPopup",
                 new { controller = "DiscountRulesHasAttribute", action = "AttributeAddPopup" },
                 new[] { "Nop.Plugin.DiscountRules.HasAttribute.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.HasAttribute.AttributeAddPopupList",
                 "Plugins/DiscountRulesHasAttribute/AttributeAddPopupList",
                 new { controller = "DiscountRulesHasAttribute", action = "AttributeAddPopupList" },
                 new[] { "Nop.Plugin.DiscountRules.HasAttribute.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.HasAttribute.LoadAttributeFriendlyNames",
                 "Plugins/DiscountRulesHasAttribute/LoadAttributeFriendlyNames",
                 new { controller = "DiscountRulesHasAttribute", action = "LoadAttributeFriendlyNames" },
                 new[] { "Nop.Plugin.DiscountRules.HasAttribute.Controllers" }
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
