﻿using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Catalog.GBS
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            
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
