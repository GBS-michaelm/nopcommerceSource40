using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Order.GBS.Factories;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using System;
using NW = Nop.Web.Controllers;

namespace Nop.Plugin.Order.GBS.Controllers
{
    public class CustomerOrderController : BasePublicController
    {
        #region Fields

        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IWebHelper _webHelper;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Constructors

        public CustomerOrderController(IOrderModelFactory orderModelFactory, IOrderService orderService, IShipmentService shipmentService, IWorkContext workContext, IOrderProcessingService orderProcessingService, IPaymentService paymentService, IPdfService pdfService, IWebHelper webHelper, RewardPointsSettings rewardPointsSettings)
        {
            this._orderModelFactory = orderModelFactory;
            this._orderService = orderService;
            this._shipmentService = shipmentService;
            this._workContext = workContext;
            this._orderProcessingService = orderProcessingService;
            this._paymentService = paymentService;
            this._pdfService = pdfService;
            this._webHelper = webHelper;
            this._rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        public IActionResult CustomerOrders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            int? page = 1;
            string status = "";
            int pageSize = 5;
            if (Request.Query.Count > 0)
            {
                page = Convert.ToInt32(Request.Query["page"]);
                status = Convert.ToString(Request.Query["status"]);
                if (Request.Query.ContainsKey("pageSize"))
                {
                    pageSize = Convert.ToInt32(Request.Query["pageSize"]);
                }
            }
            var model = _orderModelFactory.PrepareCustomerOrderListModel(status, page, pageSize);
            ViewBag.SelectedTab = status;
            TempData["SelectedTab"] = status;

            return View("~/Views/Order/CustomerOrders.cshtml", model);
        }
    }
}
