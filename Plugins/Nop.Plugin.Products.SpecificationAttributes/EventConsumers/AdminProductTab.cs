using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Products.SpecificationAttributes
{
    public class AdminProductTabConsumer : IConsumer<AdminTabStripCreated>
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;

        public AdminProductTabConsumer(ILocalizationService localizationService,
            ICustomerService customerService)
        {
            _localizationService = localizationService;
            _customerService = customerService;
        }
        
        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="tabEventInfo">The tab event information.</param>
        public void HandleEvent(AdminTabStripCreated tabEventInfo)
        {
            return;

            try
            {
                if (tabEventInfo != null && !string.IsNullOrEmpty(tabEventInfo.TabStripName))
                {
                    if (tabEventInfo.TabStripName == "product-edit")
                    {
                        var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                        object objectId = HttpContext.Current.Request.RequestContext.RouteData.Values["id"];

                        if (!string.IsNullOrEmpty(objectId.ToString()))
                        {
                            string text = _localizationService.GetResource("Plugin.Product.Artist");
                            string content = urlHelper.Action("ProductTab", "SpecificationAttributes", new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Products.SpecificationAttributes.Controllers" }, { "area", "" }, { "id", objectId } });

                            tabEventInfo.BlocksToRender.Add(new MvcHtmlString(
                            "<script>"
                                    + "$(document).ready(function() {"
                                        + "$(\"<li><a data-tab-name='tab-artist' data-toggle='tab' href='#tab-artist'>"
                                        + text
                                        + "</a></li> \").appendTo('#product-edit .nav-tabs:first');"

                                        + "$.get('" + content + "', function(result) {"
                                        + "$(\" <div class='tab-pane' id='tab-artist'>\" + result + \"</div>\").appendTo('#product-edit .tab-content:first');"
                                        + "});"
                                    + "});"
                            + "</script>"));

                        }
                    }                   
                }
            }
            catch { }
        }
    }
}
