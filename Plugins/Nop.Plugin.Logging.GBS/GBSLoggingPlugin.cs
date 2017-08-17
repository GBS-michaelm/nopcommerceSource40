using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Plugins;
using System.Web.Routing;

namespace Nop.Plugin.Logging.GBS
{
    public class GBSLoggingPlugin : BasePlugin, IMiscPlugin
    {
        //public PluginDescriptor PluginDescriptor
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }

        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public void GetConfigurationRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "LoggingConfigurationGBS";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Logging.GBS.Controllers" },
                { "area", null }
            };
            //throw new NotImplementedException();
        }

        //public void Install()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Uninstall()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
