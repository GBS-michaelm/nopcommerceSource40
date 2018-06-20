using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.BreadCrumb.GBS.Factories
{
    class GBSProductModelFactory : ProductModelFactory
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GBSProductModelFactory(IPluginFinder pluginFinder,
                IHttpContextAccessor httpContextAccessor,
                ISpecificationAttributeService specificationAttributeService,
                ICategoryService categoryService,
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
                IDateTimeHelper dateTimeHelper,
                IProductTagService productTagService,
                IAclService aclService,
                IStoreMappingService storeMappingService,
                IPermissionService permissionService,
                IDownloadService downloadService,
                IProductAttributeParser productAttributeParser,
                IDateRangeService dateRangeService,
                MediaSettings mediaSettings,
                CatalogSettings catalogSettings,
                VendorSettings vendorSettings,
                CustomerSettings customerSettings,
                CaptchaSettings captchaSettings,
                OrderSettings orderSettings,
                SeoSettings seoSettings,
                IStaticCacheManager cacheManager) :
            base(specificationAttributeService,
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
                dateTimeHelper,
                productTagService,
                aclService,
                storeMappingService,
                permissionService,
                downloadService,
                productAttributeParser,
                dateRangeService,
                mediaSettings,
                catalogSettings,
                vendorSettings,
                customerSettings,
                captchaSettings,
                orderSettings,
                seoSettings,
                cacheManager)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _cacheManager = cacheManager;
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pluginFinder = pluginFinder;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Prepare the product breadcrumb model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// Product breadcrumb model
        /// </returns>
        /// <exception cref="System.ArgumentNullException">product</exception>
        protected override ProductDetailsModel.ProductBreadcrumbModel PrepareProductBreadcrumbModel(Product product)
        {
            try
            {
                var miscPlugins = _pluginFinder.GetPlugins<BreadCrumbPlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0)
                {
                    if (product == null)
                        throw new ArgumentNullException("product");

                    var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY,
                            product.Id,
                            _workContext.WorkingLanguage.Id,
                            string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                            _storeContext.CurrentStore.Id);
                    var cachedModel = _cacheManager.Get(cacheKey, () =>
                    {
                        var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
                        {
                            Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                            ProductId = product.Id,
                            ProductName = product.GetLocalized(x => x.Name),
                            ProductSeName = product.GetSeName()
                        };

                        var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                        if (productCategories.Any())
                        {
                            var category = GetReferringCategory(_httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString(), productCategories);
                            if (category != null)
                            {
                                foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService))
                                {
                                    breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                                    {
                                        Id = catBr.Id,
                                        Name = catBr.GetLocalized(x => x.Name),
                                        SeName = catBr.GetSeName(),
                                        IncludeInTopMenu = catBr.IncludeInTopMenu
                                    });
                                }
                            }
                        }
                        return breadcrumbModel;
                    });

                    return cachedModel;
                }
                else
                {
                    return base.PrepareProductBreadcrumbModel(product);
                }
            }
            catch
            {
                return base.PrepareProductBreadcrumbModel(product);
            }
        }

        /// <summary>
        /// Gets the referring category.
        /// </summary>
        /// <param name="referringUrl">The referring URL.</param>
        /// <param name="categories">The categories.</param>
        /// <returns></returns>
        private Category GetReferringCategory(string referringUrl, IList<ProductCategory> categories)
        {
            string[] referringUrlSplit = referringUrl.Split('/');
            if (referringUrlSplit.Length > 0)
            {
                for (int i = 0; i < referringUrlSplit.Length; i++)
                {
                    if (referringUrlSplit[i].Length > 0)
                    {
                        var referringCategoryURL = referringUrlSplit[i];
                        var urlRecordService = Core.Infrastructure.EngineContext.Current.Resolve<IUrlRecordService>();
                        UrlRecord url = urlRecordService.GetBySlug(referringCategoryURL);
                        if (url != null)
                        {
                            if (url.EntityName.ToLower() == "category")
                            {
                                var referringCategory = (from c in categories where c.CategoryId == url.EntityId select c).ToList().First();
                                return referringCategory.Category;
                            }
                        }
                    }
                }
            }

            return categories[0].Category;
        }
    }
}
