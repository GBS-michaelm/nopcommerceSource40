using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using System.Linq;
using Nop.Services.Configuration;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Services.Seo;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "OrderSummaryContent")]
    public class OrderSummaryContentViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWebHelper _webHelper;
        private readonly ICcService _ccService;

        public OrderSummaryContentViewComponent(
            ISettingService settingService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductAttributeParser productAttributeParser,
            IWebHelper webHelper,
            ICcService ccService)
        {
            _settingService = settingService;
            _workContext = workContext;
            _storeContext = storeContext;
            _productAttributeParser = productAttributeParser;
            _webHelper = webHelper;
            _ccService = ccService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return OrderSummaryContentAfter();
        }

        private IViewComponentResult OrderSummaryContentAfter()
        {
            var list = new List<ICcCartScriptItem>();

            var imageModel = _ccService.GetCcCartItems(out int count);
            var returnToEditModel = GetCcCartReturnToEditItems();
            list.AddRange(imageModel.Items);
            list.AddRange(returnToEditModel.Items);
            return View("~/Plugins/Widgets.CustomersCanvas/Views/ShoppingCart/_CartScripts.cshtml", list);
        }

        private CcCartReturnToEditReplacementModel GetCcCartReturnToEditItems()
        {
            var model = new CcCartReturnToEditReplacementModel();
            var ccSettings = _settingService.LoadSetting<CcSettings>();

            var customer = _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            foreach (var shoppingCartItem in cart)
            {
                var attributeMappings =
                    _productAttributeParser.ParseProductAttributeMappings(shoppingCartItem.AttributesXml);
                foreach (var attributeMapping in attributeMappings)
                {
                    if (attributeMapping.ProductAttributeId == ccSettings.CcIdAttributeId)
                    {
                        var editCartItemUrl = Url.Action("EditorPage", "CcWidget", new { productId = shoppingCartItem.Product.Id });
                        editCartItemUrl += "&quantity=" + shoppingCartItem.Quantity + "&updateCartItemId=" + shoppingCartItem.Id;

                        var oldUrl = Url.RouteUrl("Product", new { SeName = shoppingCartItem.Product.GetSeName() });
                        oldUrl = _webHelper.ModifyQueryString(oldUrl, "updatecartitemid=" + shoppingCartItem.Id, null);

                        model.Items.Add(new CcCartReturnToEditReplacementModel.Item()
                        {
                            CartItemId = shoppingCartItem.Id,
                            OldUrl = oldUrl,
                            Url = editCartItemUrl
                        });

                    }
                }
            }
            return model;
        }

    }
}