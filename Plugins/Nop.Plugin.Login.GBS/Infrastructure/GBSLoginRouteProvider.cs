using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Login.GBS.Infrastructure
{
	public partial class GBSLoginRouteProvider : IRouteProvider
    {
        public GBSLoginRouteProvider()
        {

        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }

        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute("GBSLoginModal",
                           "gbslogin/loginmodal",
                           new { controller = "GBSLogin", action = "LoginModal" });
        }
    }
}
