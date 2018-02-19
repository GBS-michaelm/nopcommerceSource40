using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

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

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("GBSLoginModal",
                           "gbslogin/loginmodal",
                           new { controller = "GBSLogin", action = "LoginModal" },
                           new[] { "Nop.Plugin.Login.GBS.Controllers" });
        }
    }
}
