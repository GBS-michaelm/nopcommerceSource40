using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Plugins;
using System.Web.Routing;
using Nop.Core.Infrastructure;
using Nop.Core;

namespace Nop.Plugin.Checkout.GBS
{
    public class GBSCheckout :  BasePlugin, IMiscPlugin
    {       
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            //actionName = "Configure";
            //controllerName = "Checkout";
            //routeValues = new RouteValueDictionary()
            //{
            //    { "Namespaces", "Nop.Plugin.Checkout.GBS.Controllers" },
            //    { "area", null }
            //};
            throw new NotImplementedException();
        }


        public HelperClass _helper { get; set; }
        public GBSCheckout()
        {
            this._helper = new HelperClass(EngineContext.Current.Resolve<IStoreContext>(), EngineContext.Current.Resolve<IWorkContext>());
        }

        //public override void Install()
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uninstall()
        //{
        //    throw new NotImplementedException(); ////
        //}
    }
}
