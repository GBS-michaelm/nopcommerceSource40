using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Order.GBS.Factories;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Framework.Security;
using System;
using System.Web.Mvc;
using NW = Nop.Web.Controllers;

namespace Nop.Plugin.Order.GBS.Controllers
{
    public class OrderController : NW.OrderController
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

        public OrderController(IOrderModelFactory orderModelFactory, IOrderService orderService, IShipmentService shipmentService, IWorkContext workContext, IOrderProcessingService orderProcessingService, IPaymentService paymentService, IPdfService pdfService, IWebHelper webHelper, RewardPointsSettings rewardPointsSettings) : base(orderModelFactory, orderService, shipmentService, workContext, orderProcessingService, paymentService, pdfService, webHelper, rewardPointsSettings)
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

        [NopHttpsRequirement(SslRequirement.Yes)]
        public override ActionResult CustomerOrders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            int? page = 1;
            string status = "";
            int pageSize = 5;
            if (Request.QueryString.Count > 0)
            {
                page = Convert.ToInt32(Request.QueryString["page"]);
                status = Convert.ToString(Request.QueryString["status"]);
                if (Request.QueryString["pageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
                }
            }
            var model = _orderModelFactory.PrepareCustomerOrderListModel(status, page, pageSize);
            ViewBag.SelectedTab = status;
            TempData["SelectedTab"] = status;

            return View(model);
        }
    }
}
