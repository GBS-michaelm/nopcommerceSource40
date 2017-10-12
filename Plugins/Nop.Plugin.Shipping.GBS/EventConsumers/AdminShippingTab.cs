using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Shipping.GBS.Controllers;
using Nop.Core.Infrastructure;
using System;

namespace Nop.Plugin.Shipping.GBS.EventConsumers
{
   public class AdminShippingTabConsumer : IConsumer<AdminTabStripCreated>
    {
         /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="tabEventInfo">The tab event information.</param>
        public void HandleEvent(AdminTabStripCreated tabEventInfo)
        {
            if (tabEventInfo != null && !string.IsNullOrEmpty(tabEventInfo.TabStripName))
            {
                if (tabEventInfo.TabStripName == "product-edit")
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                    object objectId = HttpContext.Current.Request.RequestContext.RouteData.Values["id"];

                    if (!string.IsNullOrEmpty(objectId.ToString()))
                    {
                        string tabName = "Shipping";
                        var productController = EngineContext.Current.Resolve<ExtendedFieldsController>();
                        var tabContent = productController.AdminTab(Convert.ToInt32(objectId));

                        tabContent = HttpUtility.JavaScriptStringEncode(tabContent);
                        var str = "<script>"
                                + "$(document).ready(function() {"
                                    + "$(\"<li><a data-tab-name='tab-shipping' data-toggle='tab' href='#tab-shipping'>"
                                    + tabName
                                    + "</a></li> \").appendTo('#product-edit .nav-tabs:first');"
                                    + "$(\" <div class='tab-pane' id='tab-shipping'>" + tabContent + "</div>\")"
                                    + ".appendTo('#product-edit .tab-content:first');"
                                + "});"
                        + "</script>";                 
              


                    tabEventInfo.BlocksToRender.Add(new MvcHtmlString(str));
                    }

                }
            }
        }
    }
}

  