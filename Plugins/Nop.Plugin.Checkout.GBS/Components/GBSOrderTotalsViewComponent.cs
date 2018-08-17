using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;
using System.Linq;

namespace Nop.Plugin.Widgets.NivoSlider.Components
{
    [ViewComponent(Name = "GBSOrderTotals")]
    public class GBSOrderTotalsViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public GBSOrderTotalsViewComponent(IStoreContext storeContext, 
            IStaticCacheManager cacheManager, 
            ISettingService settingService, 
            IPictureService pictureService,
            IWorkContext workContext,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
            this._settingService = settingService;
            this._pictureService = pictureService;
            this._workContext = workContext;
            this._shoppingCartModelFactory = shoppingCartModelFactory;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var shoppingCartModel = new ShoppingCartModel();
            _shoppingCartModelFactory.PrepareShoppingCartModel(shoppingCartModel, cart,
            isEditable: false,
            prepareAndDisplayOrderReviewData: false);
            ViewBag.ShoppingCartModel = shoppingCartModel;

            var model = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, (bool)additionalData);
            return View("~/Views/Shared/Components/OrderTotals/Default.cshtml", model);
        }       
    }
}
