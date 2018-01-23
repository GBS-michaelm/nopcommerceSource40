using Nop.Admin.Models.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using System.IO;
using System.Net;
using Nop.Services.Logging;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers.Order
{
    public class CcOrderController : BasePluginController
    {
        #region fields
        private readonly ISettingService _settingService;
        private readonly ICcService _ccService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        #endregion

        #region ctor

        public CcOrderController(IOrderService orderService, ICcService ccService,
            ISettingService settingService, IProductService productService,
            ILocalizationService localizationService, ILogger logger)
        {
            _ccService = ccService;
            _orderService = orderService;
            _settingService = settingService;
            _productService = productService;
            _localizationService = localizationService;
            _logger = logger;
        }

        #endregion

        public string OrderAdminTabString(int id)
        {
            var order = _orderService.GetOrderById(id);
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
                content = Utils.GetRazorViewAsString(model,
                    "~/Plugins/Widgets.CustomersCanvas/Views/Order/OrderAdminTab.cshtml");

            return content;
        }

        public ActionResult List()
        {
            var customersCanvasSettings = _settingService.LoadSetting<CcSettings>();
            var model = new CcOrderListViewModel()
            {
                IsOrderButtonDesignButtonEnabled = customersCanvasSettings.IsOrderExportButton,
                OrderExportDesignButtonName = _localizationService.GetResource("Plugins.Widgets.CustomersCanvas.OrderExportButton")
            };
            return View(viewName: "~/Plugins/Widgets.CustomersCanvas/Views/Order/AdminList.cshtml", model: model);
        }

        // Add this appsetting to web.config in NopCommerce (Nop.Web) for disable export PDF:
        // <appSettings>
        //    <add key = "IsAllowExportOrdersPDF" value="false"/>
        // </appSettings>
        [HttpPost]
        public ActionResult Export(string selectedIds)
        {
            var allowExportOrdersPDF = System.Configuration.ConfigurationManager.AppSettings["IsAllowExportOrdersPDF"];
            if (allowExportOrdersPDF != "false")
            {
                var orders = new List<Core.Domain.Orders.Order>();
                if (selectedIds != null)
                {
                    var ids = selectedIds
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => Convert.ToInt32(x))
                        .ToArray();
                    orders.AddRange(_orderService.GetOrdersByIds(ids));
                }
                if (orders.Count > 0)
                {
                    var customersCanvasSettings = _settingService.LoadSetting<CcSettings>();
                    var fileNameTemplate = customersCanvasSettings.DesignFileName;
                    if (fileNameTemplate == null)
                    {
                        fileNameTemplate = "";
                    }
                    var path = customersCanvasSettings.OrderExportPath;
                    if (string.IsNullOrEmpty(path))
                    {
                        ErrorNotification(_localizationService.GetResource("Plugins.Widgets.CustomersCanvas.OrderExportPathWarning"));
                    }
                    else
                    {
                        var designResults = new List<Tuple<string, string, int>>();
                        foreach (var order in orders)
                        {
                            for (var i = 0; i < order.OrderItems.Count; i++)
                            {
                                var item = order.OrderItems.ElementAt(i);
                                var index = i;
                                if (order.OrderItems.Count == 1)
                                    index = -1;
                                var ccResult = _ccService.GetCcResult(item.AttributesXml);
                                if (ccResult != null)
                                {
                                    var fileName = _ccService.FormatOrderTokenString(item, fileNameTemplate);
                                    var curPath = _ccService.FormatOrderTokenString(item, path);
                                    if (!Directory.Exists(curPath))
                                    {
                                        Directory.CreateDirectory(curPath);
                                    }
                                    curPath += @"\" + fileName;
                                    if (fileNameTemplate.IndexOf("<item_index>") == -1 && fileNameTemplate.IndexOf("<item_guid>") == -1 && index != -1)
                                    {
                                        curPath += "_" + index;
                                    }
                                    curPath += ".pdf";
                                    foreach (var url in ccResult.HiResUrls)
                                    {
                                        designResults.Add(new Tuple<string, string, int>(url, curPath, item.OrderId));
                                    }
                                }
                            }
                        }
                        if (designResults.Count > 0)
                        {
                            var isError = false;
                            // TODO paraller
                            foreach (var x in designResults)
                            {
                                var url = x.Item1;
                                var filePath = x.Item2;
                                var orderId = x.Item3;
                                try
                                {
                                    new WebClient().DownloadFile(url, filePath);
                                }
                                catch (Exception ex)
                                {
                                    isError = true;
                                    _logger.Error("Error to export pdf in order id=" + orderId.ToString() + " \r\n from " + url + " \r\n to " + filePath, ex);
                                }
                            }
                            if (isError)
                            {
                                ErrorNotification(_localizationService.GetResource("Plugins.Widgets.CustomersCanvas.OrderExportDownloadError"));
                            }
                            else
                            {
                                SuccessNotification(_localizationService.GetResource("Plugins.Widgets.CustomersCanvas.OrderExportFinish"));
                            }
                        }
                        else
                        {
                            WarningNotification(_localizationService.GetResource("Plugins.Widgets.CustomersCanvas.OrderExportEmpty"));
                        }
                    }
                }
                else
                {
                    WarningNotification(_localizationService.GetResource("Admin.Orders.PdfInvoice.NoOrders"));
                }
            }
            else
            {
                WarningNotification("Export is not allowed on this site.");
            }
            return RedirectToAction("List");
        }
    }
}