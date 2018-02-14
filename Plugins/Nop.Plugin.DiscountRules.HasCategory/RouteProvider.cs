using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.HasCategory
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.HasCategory.Configure",
                 "Plugins/DiscountRulesHasCategory/Configure",
                 new { controller = "DiscountRulesHasCategory", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.HasCategory.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.HasCategory.CategoryAddPopup",
                 "Plugins/DiscountRulesHasCategory/CategoryAddPopup",
                 new { controller = "DiscountRulesHasCategory", action = "CategoryAddPopup" },
                 new[] { "Nop.Plugin.DiscountRules.HasCategory.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.HasCategory.CategoryAddPopupList",
                 "Plugins/DiscountRulesHasCategory/CategoryAddPopupList",
                 new { controller = "DiscountRulesHasCategory", action = "CategoryAddPopupList" },
                 new[] { "Nop.Plugin.DiscountRules.HasCategory.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.HasCategory.LoadCategoryFriendlyNames",
                 "Plugins/DiscountRulesHasCategory/LoadCategoryFriendlyNames",
                 new { controller = "DiscountRulesHasCategory", action = "LoadCategoryFriendlyNames" },
                 new[] { "Nop.Plugin.DiscountRules.HasCategory.Controllers" }
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
