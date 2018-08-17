using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "ProductEditorPage")]
    public class ProductEditorPageViewComponent : NopViewComponent
    {
        private readonly IProductService _productService;
        private readonly ICcService _ccService;

        public ProductEditorPageViewComponent(IProductService productService, ICcService ccService)
        {
            _productService = productService;
            _ccService = ccService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return ProductEditorPageWidget();
        }

        private IViewComponentResult ProductEditorPageWidget()
        {
            return View("~/Plugins/Widgets.CustomersCanvas/Views/ProductEditorPageWidget.cshtml");
        }

    }
}