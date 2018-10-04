using Microsoft.AspNetCore.Html;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.GBS.Controllers;
using Nop.Services.Events;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Events;
using System;

namespace Nop.Plugin.Shipping.GBS.EventConsumers
{
    public class AdminShippingTabConsumer : IConsumer<AdminTabStripCreated>
    {
         /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The tab event information.</param>
        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage != null && !string.IsNullOrEmpty(eventMessage.TabStripName))
            {
                if (eventMessage.TabStripName == "product-edit")
                {
                    var productId = ((ProductModel)eventMessage.Helper.ViewData.Model).Id;
                    if (!string.IsNullOrEmpty(productId.ToString()))
                    {
                        string tabName = "Shipping";
                        var productController = EngineContext.Current.Resolve<ExtendedFieldsController>();
                        var tabContent = productController.AdminTab(Convert.ToInt32(productId), eventMessage.Helper);

                        var str = "<script>"
                                + "$(document).ready(function() {"
                                    + "$(\"<li><a data-tab-name='tab-shipping' data-toggle='tab' href='#tab-shipping'>"
                                    + tabName
                                    + "</a></li> \").appendTo('#product-edit .nav-tabs:first');"
                                    + "$(\" <div class='tab-pane' id='tab-shipping'>" + tabContent + "</div>\")"
                                    + ".appendTo('#product-edit .tab-content:first');"
                                + "});"
                        + "</script>";                 
              


                    eventMessage.BlocksToRender.Add(new HtmlString(str));
                    }

                }
            }
        }
    }
}

  