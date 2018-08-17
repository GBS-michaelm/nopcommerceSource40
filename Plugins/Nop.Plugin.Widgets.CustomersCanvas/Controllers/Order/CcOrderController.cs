using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Web.Framework.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using System.IO;
using System.Net;
using Nop.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers.Order
{
    [Area(AreaNames.Admin)]
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

        public CcOrderController(
            IOrderService orderService, 
            ICcService ccService,
            ISettingService settingService, 
            IProductService productService,
            ILocalizationService localizationService, 
            ILogger logger
            )
        {
            _ccService = ccService;
            _orderService = orderService;
            _settingService = settingService;
            _productService = productService;
            _localizationService = localizationService;
            _logger = logger;
            // ignore cc ssl problems
            ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };
        }

        #endregion
        
        // TODO protect h2
        // Add this appsetting to web.config in NopCommerce (Nop.Web) for disable export PDF:
        // <appSettings>
        //    <add key = "IsAllowExportOrdersPDF" value="false"/>
        // </appSettings>
        [HttpPost]
        public ActionResult Export(string selectedIds)
        {
            var allowExportOrdersPDF = "true";
            //System.Configuration.ConfigurationManager.AppSettings["IsAllowExportOrdersPDF"];
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
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}