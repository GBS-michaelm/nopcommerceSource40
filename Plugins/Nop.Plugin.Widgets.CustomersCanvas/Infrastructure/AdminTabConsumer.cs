using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Controllers;
using Nop.Plugin.Widgets.CustomersCanvas.Controllers.Order;
using Nop.Services.Events;
using Nop.Web.Framework.Events;
using System;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core;

namespace Nop.Plugin.Widgets.CustomersCanvas.Infrastructure
{
    public class AdminTabConsumer : IConsumer<AdminTabStripCreated>
    {
        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage.TabStripName == "product-edit")
            {
                object objectId = HttpContext.Current.Request.RequestContext.RouteData.Values["id"];

                if (!string.IsNullOrEmpty(objectId.ToString()))
                {
                    string tabName = "Personalization Settings";

                    var productController = EngineContext.Current.Resolve<CcProductController>();
                    var tabContent = productController.ProductAdminTabString(Convert.ToInt32(objectId));
                    tabContent = HttpUtility.JavaScriptStringEncode(tabContent);
                    var str = "<script>"
                            + "$(document).ready(function() {"
                                + "$(\"<li><a data-tab-name='tab-name' data-toggle='tab' href='#tab-name'>"
                                + tabName
                                + "</a></li> \").appendTo('#product-edit .nav-tabs:first');"
                                + "$(\" <div class='tab-pane' id='tab-name'>" + tabContent + "</div>\")"
                                + ".appendTo('#product-edit .tab-content:first');"
                            + "});"
                    + "</script>";

                    var colorTabName = "Color settings";
                    var colorTabContent = productController.ProductAdminTabColorSetting(Convert.ToInt32(objectId));
                    if (!string.IsNullOrEmpty(colorTabContent))
                    {
                        colorTabContent = HttpUtility.JavaScriptStringEncode(colorTabContent);
                        str += "<script>"
                                + "$(document).ready(function() {"
                                    + "$(\"<li><a data-tab-name='tab-name' data-toggle='tab' href='#tab-color'>"
                                    + colorTabName
                                    + "</a></li> \").appendTo('#product-edit .nav-tabs:first');"
                                    + "$(\" <div class='tab-pane' id='tab-color'>" + colorTabContent + "</div>\")"
                                    + ".appendTo('#product-edit .tab-content:first');"
                                + "});"
                        + "</script>";
                    }
                    eventMessage.BlocksToRender.Add(new MvcHtmlString(str));
                }
            }
            else if (eventMessage.TabStripName == "order-edit")
            {
                object objectId = HttpContext.Current.Request.RequestContext.RouteData.Values["id"];

                if (!string.IsNullOrEmpty(objectId.ToString()))
                {
                    string tabName = "Designs";

                    var orderController = EngineContext.Current.Resolve<CcOrderController>();

                    var tabContent = orderController.OrderAdminTabString(Convert.ToInt32(objectId));

                    if (!string.IsNullOrEmpty(tabContent))
                    {
                        tabContent = HttpUtility.JavaScriptStringEncode(tabContent);
                        var str = "<script>"
                                + "$(document).ready(function() {"
                                    + "$(\"<li><a data-tab-name='tab-name' data-toggle='tab' href='#tab-name'>"
                                    + tabName
                                    + "</a></li> \").appendTo('#order-edit .nav-tabs:first');"
                                    + "$(\" <div class='tab-pane' id='tab-name'>" + tabContent + "</div>\")"
                                    + ".appendTo('#order-edit .tab-content:first');"
                                + "});"
                        + "</script>";

                        eventMessage.BlocksToRender.Add(new MvcHtmlString(str));
                    }
                }
            }
        }

    }
}