using Nop.Core.Domain.Payments;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Serialization;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Controllers;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Widgets.CustomersCanvas.Controllers;
using Nop.Plugin.Widgets.CustomersCanvas;
using Nop.Plugin.CanvasOverride.GBS.DataAccess;
using System.Data;

namespace Nop.Plugin.Widgets.CanvasOverride.GBS.Controllers
{
    public class CanvasOverrideController : CcWidgetController
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
        #endregion

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
            IDownloadService downloadService) : base (
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
        #endregion

        public PartialViewResult ProductDetailsOverride(EditorPageModel editorPageModel)
        {
            DBManager db = new DBManager();
            DataTable dt = new DataTable();
            dt.Columns.Add("Sku", typeof(string));
            
            return base.ProductDetailsAfterBreadcrumb(editorPageModel);
        }

    }
}
