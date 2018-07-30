using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Custom.Orders;
using Nop.Web.Framework.Components;
using System;
using static Nop.Plugin.Order.GBS.Orders.OrderExtensions;
using Nop.Core.Infrastructure;
using Nop.Plugin.Order.GBS.Orders;

namespace Nop.Plugin.Order.GBS.Components
{
	[ViewComponent(Name = "DisplayLegacyOrderItemImage")]
    public class DisplayLegacyOrderItemImageComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreContext _storeContext;
		private readonly IGBSOrderService _gbsOrderService;

		public DisplayLegacyOrderItemImageComponent(IHttpContextAccessor httpContextAccessor,
            IStoreContext storeContext,
            IGBSOrderService gbsOrderService)
        {
            this._httpContextAccessor = httpContextAccessor;
            _storeContext = storeContext;
            this._gbsOrderService = gbsOrderService; //EngineContext.Current.Resolve<GBSOrderService>();
		}


        public IViewComponentResult Invoke(object additionalData = null)
        {
			if (additionalData == null) { return null; }

			//Nop.Core.Domain.Orders.OrderItem item = ((Nop.Services.Custom.Orders.GBSOrderService)_orderService).GetOrderItemById(Convert.ToInt32(additionalData),true);
			//if (item != null) { return null; }
			//LegacyOrderItem orderItem = (LegacyOrderItem)new Nop.Plugin.Order.GBS.Orders.OrderExtensions().GetOrderItemById(Convert.ToInt32(additionalData), true);
			//string cString = "<img title = '' alt = '"+ orderItem.Product.Name+"' src = '"+orderItem.legacyPicturePath+"'>";
			//return Content(cString);
			//Nop.Core.Domain.Orders.OrderItem item = ((Nop.Services.Custom.Orders.GBSOrderService)_orderService).GetOrderItemById(Convert.ToInt32(additionalData));
			Nop.Core.Domain.Orders.OrderItem item = _gbsOrderService.GetOrderItemById(Convert.ToInt32(additionalData));


			if (item is LegacyOrderItem)
			{
				LegacyOrderItem orderItem = (LegacyOrderItem)item;

				string cString = "<img title = '' alt = '" + orderItem.Product.Name + "' src = '" + orderItem.legacyPicturePath + "'>";
				return Content(cString);
			}
			else
			{
				return null;
			}
		}
    }
}
