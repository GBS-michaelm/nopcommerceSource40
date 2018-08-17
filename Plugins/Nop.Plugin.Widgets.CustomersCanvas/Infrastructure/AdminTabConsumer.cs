using Nop.Services.Events;
using Nop.Web.Framework.Events;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Services.Configuration;
using Nop.Services.Catalog;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Extensions;
using Nop.Services.Orders;

namespace Nop.Plugin.Widgets.CustomersCanvas.Infrastructure
{
    public class AdminTabConsumer : IConsumer<AdminTabStripCreated>
    {
        #region fields
        private readonly ISettingService _settingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICcService _ccService;
        private readonly IOrderService _orderService;
        #endregion
        #region ctor
        public AdminTabConsumer(
            ISettingService settingService,
            ISpecificationAttributeService specificationAttributeService,
            ICcService ccService,
            IOrderService orderService)
        {
            _settingService = settingService;
            _specificationAttributeService = specificationAttributeService;
            _ccService = ccService;
            _orderService = orderService;
        }
        #endregion
        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage.TabStripName == "product-edit")
            {
                var productId = ((ProductModel)eventMessage.Helper.ViewData.Model).Id;

                if (!string.IsNullOrEmpty(productId.ToString()))
                {
                    string tabName = "Personalization Settings";

                    var tabContent = ProductAdminTabString(productId, eventMessage.Helper);

                    var tabElementId = "au-product-editor-setting";
                    var productEditorTab = new HtmlString($@"
                    <script type='text/javascript'>
                        $(document).ready(function() {{
                            $(`
                                <li>
                                    <a data-tab-name='{tabElementId}' data-toggle='tab' href='#{tabElementId}'>
                                        {tabName}
                                    </a>
                                </li>
                            `).appendTo('#product-edit > ul.nav-tabs.nav');
                            $(`
                                <div class='tab-pane' id='{tabElementId}'>
                                    {tabContent}
                                </div>
                            `).appendTo('#product-edit .tab-content:first');
                        }});
                    </script>");
                    eventMessage.BlocksToRender.Add(productEditorTab);

                    #region TODO GBS
                    //var colorTabName = "Color settings";
                    //var colorTabContent = productController.ProductAdminTabColorSetting(Convert.ToInt32(objectId));
                    //if (!string.IsNullOrEmpty(colorTabContent))
                    //{
                    //    colorTabContent = JavaScriptEncoder.Default.Encode(colorTabContent);
                    //    str += "<script>"
                    //            + "$(document).ready(function() {"
                    //                + "$(\"<li><a data-tab-name='tab-name' data-toggle='tab' href='#tab-color'>"
                    //                + colorTabName
                    //                + "</a></li> \").appendTo('#product-edit .nav-tabs:first');"
                    //                + "$(\" <div class='tab-pane' id='tab-color'>" + colorTabContent + "</div>\")"
                    //                + ".appendTo('#product-edit .tab-content:first');"
                    //            + "});"
                    //    + "</script>";
                    //}
                    #endregion
                }
            }
            else if (eventMessage.TabStripName == "order-edit")
            {
                var orderId = ((OrderModel)eventMessage.Helper.ViewData.Model).Id;
                if (!string.IsNullOrEmpty(orderId.ToString()))
                {
                    string tabName = "Designs";

                    var tabContent = OrderAdminTabString(orderId, eventMessage.Helper);

                    if (!string.IsNullOrEmpty(tabContent))
                    {
                        var tabElementId = "au-order-design-results";
                        var orderDesignTab = new HtmlString($@"
                        <script type='text/javascript'>
                            $(document).ready(function() {{
                                $(`
                                    <li>
                                        <a data-tab-name='{tabElementId}' data-toggle='tab' href='#{tabElementId}'>
                                            {tabName}
                                        </a>
                                    </li>
                                `).appendTo('#order-edit > ul.nav-tabs.nav');
                                $(`
                                    <div class='tab-pane' id='{tabElementId}'>
                                        {tabContent}
                                    </div>
                                `).appendTo('#order-edit  .tab-content:first');
                            }});
                        </script>");

                        eventMessage.BlocksToRender.Add(orderDesignTab);
                    }
                }
            }
        }

        private string ProductAdminTabString(int productId, IHtmlHelper helper)
        {
            var settings = _settingService.LoadSetting<CcSettings>();
            var specAttrs = _specificationAttributeService.GetProductSpecificationAttributes(productId);
            var editorName = "";
            var editorSpecAttr = specAttrs.FirstOrDefault(x => x.SpecificationAttributeOption.SpecificationAttributeId == settings.EditorDefinitionSpecificationAttributeId);
            if (editorSpecAttr != null)
            {
                editorName = editorSpecAttr.CustomValue;
            }
            var configName = "";
            var configSpecattr = specAttrs.FirstOrDefault(x => x.SpecificationAttributeOption.SpecificationAttributeId == settings.EditorConfigurationSpecificationAttributeId);
            if (configSpecattr != null)
            {
                configName = configSpecattr.CustomValue;
            }

            var model = new CcProductSetting
            {
                ProductId = productId,
                EditorSpecAttrId = settings.EditorDefinitionSpecificationAttributeId,
                EditorSpecAttrOptionId = settings.EditorDefinitionSpecificationOptionId,
                EditorConfigSpecAttrId = settings.EditorConfigurationSpecificationAttributeId,
                EditorConfigSpecAttrOptionId = settings.EditorConfigurationSpecificationOptionId,
                EditorList = _ccService.GetAllEditors(),
                EditorName = editorName,
                ConfigName = configName,
                CcUrl = settings.ServerHostUrl
            };
            var tabContent = helper.Partial("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAdminTab.cshtml", model).RenderHtmlContent();
            tabContent = JavaScriptEncoder.Default.Encode(tabContent);
            return tabContent;
        }

        private string OrderAdminTabString(int orderId, IHtmlHelper helper)
        {
            var order = _orderService.GetOrderById(orderId);
            var setting = _settingService.LoadSetting<CcSettings>();
            var fileNameTemplate = setting.DesignFileName;
            if (fileNameTemplate == null)
            {
                fileNameTemplate = "";
            }

            var flag = false;
            var model = new CcOrderModel();
            foreach (var item in order.OrderItems)
            {
                model.Items.Add(item);
                var ccResult = _ccService.GetCcResult(item.AttributesXml);
                if (ccResult != null)
                {
                    var fileName = _ccService.FormatOrderTokenString(item, fileNameTemplate);
                    ccResult.HiResOutputName = fileName;
                    model.CcResult.Add(ccResult);
                    flag = true;
                }
            }
            var content = "";
            if (flag)
            {
                content = helper.Partial("~/Plugins/Widgets.CustomersCanvas/Views/Order/OrderAdminTab.cshtml", model).RenderHtmlContent();
                content = JavaScriptEncoder.Default.Encode(content);
            }
            return content;
        }
    }
}