using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Products.SpecificationAttributes
{
	public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute("Plugin.Products.SpecificationAttributes.ProductTab",
               "plugins/artist/producttab",
               new { controller = "SpecificationAttributes", action = "ProductTab" });

            routes.MapRoute("Plugin.Products.SpecificationAttributes.ProductTabPost",
               "plugins/artist/producttabpost",
               new { controller = "SpecificationAttributes", action = "ProductTabPost" });

            routes.MapRoute("Plugin.Products.SpecificationAttributes.DetailPage",
              "artistdetail",
              new { controller = "SpecificationAttributes", action = "ArtistCategory" });
            

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
