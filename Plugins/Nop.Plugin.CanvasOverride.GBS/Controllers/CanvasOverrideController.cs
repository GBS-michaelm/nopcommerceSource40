using Nop.Core;
using Nop.Plugin.Widgets.CustomersCanvas;
using Nop.Plugin.Widgets.CustomersCanvas.Controllers;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Controllers;

namespace Nop.Plugin.CanvasOverride.GBS.Controllers
{
    public partial class CanvasOverrideController : CcWidgetController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICcService _ccService;
        private readonly IShoppingCartService _cartService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly CcSettings _customersCanvasSettings;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ProductController _productController;
        private readonly ShoppingCartController _shoppingCartController;
        private readonly IDownloadService _downloadService;

        #endregion Fields

        #region ctor

        public CanvasOverrideController(IProductService productService,
            IOrderService orderService,
            ICcService ccService,
            IShoppingCartService cartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductAttributeService productAttributeService,
            ISettingService settingService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IDownloadService downloadService) : base(
                productService,
                orderService,
                ccService,
                cartService,
                workContext,
                storeContext,
                productAttributeService,
                settingService,
                pictureService,
                productAttributeParser,
                shoppingCartService,
                webHelper,
                specificationAttributeService,
                downloadService
                )
        {

        }

        #endregion ctor
    }
}