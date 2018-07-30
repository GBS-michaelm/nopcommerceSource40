using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Common;
using System.Collections.Generic;

namespace Nop.Plugin.Order.GBS
{
	public class MyOrderServicePlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
		private readonly IWebHelper _webHelper;

		public MyOrderServicePlugin(IWebHelper webHelper)
		{
			_webHelper = webHelper;
		}

		public override string GetConfigurationPageUrl()
		{
			return _webHelper.GetStoreLocation() + "Admin/ConfigurationGBS/Configure";
		}

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        //public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        //{
        //    if (widgetZone == "order_addPhoneNumber_widget") 
        //    {
        //            actionName = "AddPhoneNumber";
        //            controllerName = "GBSOrder";
        //            routeValues = new RouteValueDictionary()
        //            {
        //                { "Namespaces", "Nop.Plugin.Order.GBS.Controllers" },
        //                { "area", null }
        //            };
        //    } else 
        //    {
        //        actionName = "DisplayLegacyOrderItemImage";
        //        controllerName = "GBSOrder";
        //        routeValues = new RouteValueDictionary()
        //            {
        //                { "Namespaces", "Nop.Plugin.Order.GBS.Controllers" },
        //                { "area", "" }
        //            };
        //    }
        //}

		public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
		{
			if (widgetZone == "order_addPhoneNumber_widget")
			{
				viewComponentName = "AddPhoneNumber";				
			}
			else
			{
				viewComponentName = "DisplayLegacyOrderItemImage";
			}
		}

		public IList<string> GetWidgetZones()
        {
            return new List<string> { "order_addPhoneNumber_widget", "orderdetails_product_line","orderdetails_product_line_legacy" }; 
        }
    }
}
