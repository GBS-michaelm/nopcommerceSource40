using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Products.SpecificationAttributes
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Products.SpecificationAttributes.ProductTab",
               "plugins/artist/producttab",
               new { controller = "SpecificationAttributes", action = "ProductTab" },
               new[] { "Nop.Plugin.Products.SpecificationAttributes.Controllers" });

            routes.MapRoute("Plugin.Products.SpecificationAttributes.ProductTabPost",
               "plugins/artist/producttabpost",
               new { controller = "SpecificationAttributes", action = "ProductTabPost" },
               new[] { "Nop.Plugin.Products.SpecificationAttributes.Controllers" });

            routes.MapRoute("Plugin.Products.SpecificationAttributes.DetailPage",
              "artistdetail",
              new { controller = "SpecificationAttributes", action = "ArtistCategory" },
              new[] { "Nop.Plugin.Products.SpecificationAttributes.Controllers" });


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
