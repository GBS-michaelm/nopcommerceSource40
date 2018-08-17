using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Plugin.Widgets.CustomersCanvas.Services;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "OrderDetails")]
    public class OrderDetailsViewComponent : NopViewComponent
    {
        private readonly IOrderService _orderService;
        private readonly ICcService _ccService;
       
        public OrderDetailsViewComponent(
            IOrderService orderService,
            ICcService ccService)
        {
            _orderService = orderService;
            _ccService = ccService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (additionalData != null && additionalData is int)
            {
                return OrderItemDetailsCcResult((int)additionalData);
            }
            return Content("");
        }

        private IViewComponentResult OrderItemDetailsCcResult(int id)
        {
            var item = _orderService.GetOrderItemById(id);
            var ccResultData = _ccService.GetCcResult(item.AttributesXml);
            if (ccResultData != null)
            {
                if (item.Order.PaymentStatus != PaymentStatus.Paid || item.Order.OrderStatus != OrderStatus.Complete)
                {
                    ccResultData.HiResUrls = null;
                }
                ccResultData.ReturnToEditUrl = null;
                ccResultData.HiResOutputName = item.Product.Name;
            }
            return View("~/Plugins/Widgets.CustomersCanvas/Views/Order/_CcResult.cshtml", ccResultData);
        }

    }
}
