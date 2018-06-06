using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
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
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Factories;
using Nop.Plugin.Catalog.GBS.DataAccess;
using Nop.Core.Plugins;
using Nop.Plugin.BusinessLogic.GBS.Factories;

namespace Nop.Plugin.Catalog.GBS.Factories
{
    /// <summary>
    /// Represents the product model factory
    /// </summary>
    public partial class ProductModelFactory : Nop.Web.Factories.ProductModelFactory
    {
        #region Fields
        
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductTagService _productTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDateRangeService _dateRangeService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IPluginFinder _pluginFinder;
        #endregion

        #region Constructors

        public ProductModelFactory(ISpecificationAttributeService specificationAttributeService,
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
            SeoSettings seoSettings,
            ICacheManager cacheManager,
            IPluginFinder pluginFinder) :base (
                     specificationAttributeService,
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
                     seoSettings,
                     cacheManager
                )
        {
            this._specificationAttributeService = specificationAttributeService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._productTemplateService = productTemplateService;
            this._productAttributeService = productAttributeService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._productTagService = productTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._productAttributeParser = productAttributeParser;
            this._dateRangeService = dateRangeService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._seoSettings = seoSettings;
            this._cacheManager = cacheManager;
            this._pluginFinder = pluginFinder;
        }

        #endregion


        #region Methods


        /// <summary>
        /// Prepare the product overview models
        /// </summary>
        /// <param name="products">Collection of products</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Collection of product overview model</returns>
        //public override IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
        //    bool preparePriceModel = true, bool preparePictureModel = true,
        //    int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
        //    bool forceRedirectionAfterAddingToCart = false)
        //{
        //    var models = base.PrepareProductOverviewModels(products, preparePriceModel, preparePictureModel, productThumbPictureSize, prepareSpecificationAttributes, forceRedirectionAfterAddingToCart);

        //    foreach(var model in models)
        //    {
        //        if (string.IsNullOrEmpty(model.DefaultPictureModel.ImageUrl))
        //        {
        //            Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
        //            paramDicEx.Add("@categoryId", model.b);
        //            paramDicEx.Add("@sku", model.Sku);

        //            DBManager manager = new DBManager();
        //            string select = "EXEC usp_GetClassicImage @categoryId, @sku";
        //            var result = (string)manager.GetParameterizedScalar(select, paramDicEx);
        //            model.DefaultPictureModel.ImageUrl = result;
        //            model.DefaultPictureModel.FullSizeImageUrl = result;
        //            model.DefaultPictureModel.ThumbImageUrl = result;
        //        }
        //    }
        //    return models;
        //}

        /// <summary>
        /// Prepare the product details model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>Product details model</returns>
        public override ProductDetailsModel PrepareProductDetailsModel(Product product,
            ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            var modelFactories = Nop.Core.Infrastructure.EngineContext.Current.ResolveAll<IProductModelFactory>();
            var foxModel =  modelFactories.Where(x => x.GetType().FullName.Contains("FoxNetSoft.Plugin.Misc.MSSQLProvider")).FirstOrDefault();
            var model = new ProductDetailsModel();
            if (foxModel != null)
            {
                 model = foxModel.PrepareProductDetailsModel(product, updatecartitem, isAssociatedProduct);

            }
            else
            {
                 model = base.PrepareProductDetailsModel(product, updatecartitem, isAssociatedProduct);
            }
            var miscPlugins = _pluginFinder.GetPlugins<CategoryNavigationProvider>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                var picModels = _pictureService.GetPicturesByProductId(product.Id);
                if (picModels == null || picModels.Count == 0)
                {
                    var result = string.Empty;
                    var catId = 0;
                    if (model.Breadcrumb.CategoryBreadcrumb.Count > 0)
                    {
                         catId = model.Breadcrumb.CategoryBreadcrumb.LastOrDefault().Id;
                         result = Helpers.GetPictureUrl(catId, model.Sku);
                    }
                    if (!string.IsNullOrEmpty(result))
                    {
                        model.DefaultPictureModel.ImageUrl = result;
                        model.DefaultPictureModel.FullSizeImageUrl = result;
                        model.DefaultPictureModel.ThumbImageUrl = result;
                    }
                }
            }
            return model;
        }

        ///// <summary>
        ///// Prepare the product details picture model
        ///// </summary>
        ///// <param name="product">Product</param>
        ///// <param name="isAssociatedProduct">Whether the product is associated</param>
        ///// <returns>Picture model for the default picture and list of picture models for all product pictures</returns>
        //protected override dynamic PrepareProductDetailsPictureModel(Product product, bool isAssociatedProduct = false)
        //{
        //    var model = base.PrepareProductDetailsPictureModel(product, isAssociatedProduct);
        //    var miscPlugins = _pluginFinder.GetPlugins<CategoryNavigationProvider>(storeId: _storeContext.CurrentStore.Id).ToList();
        //    if (miscPlugins.Count > 0)
        //    {
        //        var picModels = _pictureService.GetPicturesByProductId(product.Id);
        //        if (picModels == null || picModels.Count == 0)
        //        {
        //            var catId = model.Breadcrumb.CategoryBreadcrumb.LastOrDefault().Id;

        //            var result = Helpers.GetPictureUrl(catId, model.Sku);
        //            model.DefaultPictureModel.ImageUrl = result;
        //            model.DefaultPictureModel.FullSizeImageUrl = result;
        //            model.DefaultPictureModel.ThumbImageUrl = result;
        //        }
        //    }
        //    return model;
        //}

        /// <summary>
        /// Prepare the product tier price models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of tier price model</returns>
        protected override IList<ProductDetailsModel.TierPriceModel> PrepareProductTierPriceModels(Product product)
        {

            if (product == null)
                throw new ArgumentNullException("product");
            try
            {
                var model = product.TierPrices.OrderBy(x => x.Quantity)
                       .FilterByStore(_storeContext.CurrentStore.Id)
                       .FilterForCustomer(_workContext.CurrentCustomer)
                       .FilterByDate()
                       .RemoveDuplicatedQuantities()
                       .Select(tierPrice =>
                       {
                           decimal taxRate;
                           var priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product,
                               _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out taxRate);
                           var price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);

                           if (Decimal.Round(price, 2) != price)
                           {
                               return new ProductDetailsModel.TierPriceModel
                               {
                                   Quantity = tierPrice.Quantity,
                                   Price = "$"+price.ToString("N3")
                               };
                           }
                           return new ProductDetailsModel.TierPriceModel
                           {
                               Quantity = tierPrice.Quantity,
                               Price = _priceFormatter.FormatPrice(price, false, false)
                           };
                       }).ToList();

                return model;
            }
            catch (Exception ex)
            {
                return base.PrepareProductTierPriceModels(product);
            }
        }


        /// <summary>
        /// Prepare the product breadcrumb model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product breadcrumb model</returns>
        protected override ProductDetailsModel.ProductBreadcrumbModel PrepareProductBreadcrumbModel(Product product)
        {
            try
            {
                var miscPlugins = _pluginFinder.GetPlugins<CategoryNavigationProvider>(storeId: _storeContext.CurrentStore.Id).ToList();
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
                            var category = GetReferringCategory(HttpContext.Current.Request.UrlReferrer.AbsolutePath, productCategories);
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
            catch (Exception ex)
            {
                return base.PrepareProductBreadcrumbModel(product);
            }
        }

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
                        var urlRecordService = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IUrlRecordService>();
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

    #endregion
}
