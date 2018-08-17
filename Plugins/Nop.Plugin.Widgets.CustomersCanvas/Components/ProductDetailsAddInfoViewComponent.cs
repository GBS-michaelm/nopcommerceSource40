using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Microsoft.AspNetCore.Routing;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "ProductDetailsAddInfo")]
    public class ProductDetailsAddInfoViewComponent : NopViewComponent
    {
        private readonly IProductService _productService;
        private readonly ICcService _ccService;

        public ProductDetailsAddInfoViewComponent(IProductService productService, ICcService ccService)
        {
            _productService = productService;
            _ccService = ccService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var data = 0;
            if (additionalData != null && additionalData is int)
            {
                data = (int)additionalData;
            }
            else
            {
                var reqData = Request.HttpContext.GetRouteData().Values["productid"];
                if (reqData != null)
                {
                    data = (int)reqData;
                }
            }
            if (data != 0)
            {
                return ChangeAddToCartButton(data);
            }
            return Content("");
        }

        private IViewComponentResult ChangeAddToCartButton(int id)
        {
            var isProductForCc = _ccService.IsProductForCc(id);
            if (isProductForCc)
            {
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/_ChangeAddToCartButton.cshtml", id);
            }
            else
            {
                return Content("");
            }
        }

    }
}