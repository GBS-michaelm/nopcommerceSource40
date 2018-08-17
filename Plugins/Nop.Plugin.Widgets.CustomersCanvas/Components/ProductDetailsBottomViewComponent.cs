using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using System.Linq;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "ProductDetailsBottom")]
    public class ProductDetailsBottomViewComponent : NopViewComponent
    {
        private readonly IProductService _productService;
        private readonly ICcService _ccService;

        public ProductDetailsBottomViewComponent(
            IProductService productService,
            ICcService ccService)
        {
            _productService = productService;
            _ccService = ccService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (additionalData != null && additionalData is int)
            {
                return HideAttributes((int)additionalData);
            }
            return Content("");
        }

        private IViewComponentResult HideAttributes(int id)
        {
            var isProductForCc = _ccService.IsProductForCc(id);
            if (isProductForCc)
            {
                var product = _productService.GetProductById(id);
                var mapping = product.ProductAttributeMappings;
                var ccOptionNames = _ccService.GetCcOptionsFromConfig(id);
                var optionsIdList = new List<int>();
                foreach (var optionName in ccOptionNames)
                {
                    var option = mapping.FirstOrDefault(x => x.ProductAttribute.Name == optionName);
                    if (option != null)
                    {
                        optionsIdList.Add(option.Id);
                    }
                }
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/_HideAttributes.cshtml", optionsIdList);
            }
            return Content("");
        }

    }
}

