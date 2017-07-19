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

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers.Order
{
    public class CcOrderController : BasePluginController
    {
        #region fields

        private readonly ICcService _ccService;
        private readonly IOrderService _orderService;
        private readonly Admin.Controllers.OrderController _orderController;

        #endregion

        #region ctor

        public CcOrderController(IOrderService orderService,
            ICcService ccService)
        {
            _orderController = EngineContext.Current.Resolve<Admin.Controllers.OrderController>();
            _ccService = ccService;
            _orderService = orderService;
        }

        #endregion

        public string OrderAdminTabString(int id)
        {
            var order = _orderService.GetOrderById(id);

            var oldStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.ShippingNotRequired;
            _orderService.UpdateOrder(order);

            var actionResult = _orderController.Edit(id);
            ViewResult viewResult = (ViewResult) actionResult;

            order.ShippingStatus = oldStatus;
            _orderService.UpdateOrder(order);

            var orderModel = (OrderModel) viewResult.ViewData.Model;
            var flag = false;
            var model = new CcOrderModel();
            for (var i = 0; i < orderModel.Items.Count; i++)
            {
                var item = orderModel.Items[i];
                model.Items.Add(item);
                var ccResult = _ccService.GetCcResult(order.OrderItems.ElementAt(i).AttributesXml);
                model.CcResult.Add(ccResult);
                if (ccResult != null)
                    flag = true;
            }
            var content = "";
            if (flag)
                content = Utils.GetRazorViewAsString(model,
                    "~/Plugins/Widgets.CustomersCanvas/Views/Order/OrderAdminTab.cshtml");

            return content;
        }

    }
}