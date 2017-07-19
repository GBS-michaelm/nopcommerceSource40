using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;
using System.Web;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers.Product
{
    public class ProductModelController : ProductController
    {
        #region ctor
        private readonly IProductService _productService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        public ProductModelController(ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IProductTemplateService productTemplateService,
            IProductAttributeService productAttributeService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IDateTimeHelper dateTimeHelper,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            ICompareProductsService compareProductsService,
            IWorkflowMessageService workflowMessageService,
            IProductTagService productTagService,
            IOrderReportService orderReportService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICustomerActivityService customerActivityService,
            IProductAttributeParser productAttributeParser,
            IShippingService shippingService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            SeoSettings seoSettings,
            ICacheManager cacheManager) : base(
                categoryService,
                manufacturerService,
                productService,
                vendorService,
                productTemplateService,
                productAttributeService,
                workContext,
                storeContext,
                taxService,
                currencyService,
                pictureService,
                localizationService,
                measureService,
                priceCalculationService,
                priceFormatter,
                webHelper,
                specificationAttributeService,
                dateTimeHelper,
                recentlyViewedProductsService,
                compareProductsService,
                workflowMessageService,
                productTagService,
                orderReportService,
                aclService,
                storeMappingService,
                permissionService,
                downloadService,
                customerActivityService,
                productAttributeParser,
                shippingService,
                eventPublisher,
                mediaSettings,
                catalogSettings,
                vendorSettings,
                shoppingCartSettings,
                localizationSettings,
                customerSettings,
                captchaSettings,
                seoSettings, cacheManager)
        {
            this._productService = productService;
            this._specificationAttributeService = specificationAttributeService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
        }

        #endregion

        public ProductDetailsModel GetProductModel(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return null;

            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);

                if (product.Id != updatecartitem.ProductId)
                {
                    updatecartitem = null;
                }
            }

            var model = base.PrepareProductDetailsPageModel(product, updatecartitem);

            // add hidden specification params
            // which checked as "don't show on product page"
            model.ProductSpecifications =
                _specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, null)
               .Select(psa =>
               {
                   var m = new ProductSpecificationModel
                   {
                       SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                       SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                       ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb
                   };

                   switch (psa.AttributeType)
                   {
                       case SpecificationAttributeType.Option:
                           m.ValueRaw = HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                           break;
                       case SpecificationAttributeType.CustomText:
                           m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
                           break;
                       case SpecificationAttributeType.CustomHtmlText:
                           m.ValueRaw = psa.CustomValue;
                           break;
                       case SpecificationAttributeType.Hyperlink:
                           m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
                           break;
                       default:
                           break;
                   }
                   return m;
               }).ToList();

            return model;
        }

    }
}
