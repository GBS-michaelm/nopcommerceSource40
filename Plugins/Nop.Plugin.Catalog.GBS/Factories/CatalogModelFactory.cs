using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Events;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Web.Factories;
using Nop.Plugin.Catalog.GBS.DataAccess;
using Nop.Core.Plugins;

namespace Nop.Plugin.Catalog.GBS.Factories
{
    public partial class CatalogModelFactory : Nop.Web.Factories.CatalogModelFactory
    {
        #region Fields

        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductTagService _productTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITopicService _topicService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISearchTermService _searchTermService;
        private readonly HttpContextBase _httpContext;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICacheManager _cacheManager;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
        private readonly IPluginFinder _pluginFinder;

        #endregion

        #region Constructors

        public CatalogModelFactory(IProductModelFactory productModelFactory,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IProductTagService productTagService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ITopicService topicService,
            IEventPublisher eventPublisher,
            ISearchTermService searchTermService,
            HttpContextBase httpContext,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            BlogSettings blogSettings,
            ForumSettings forumSettings,
            ICacheManager cacheManager,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            IPluginFinder pluginFinder) : base(
                     productModelFactory,
                     categoryService,
                     manufacturerService,
                     productService,
                     vendorService,
                     categoryTemplateService,
                     manufacturerTemplateService,
                     workContext,
                     storeContext,
                     currencyService,
                     pictureService,
                     localizationService,
                     priceFormatter,
                     webHelper,
                     specificationAttributeService,
                     productTagService,
                     aclService,
                     storeMappingService,
                     topicService,
                     eventPublisher,
                     searchTermService,
                     httpContext,
                     mediaSettings,
                     catalogSettings,
                     vendorSettings,
                     blogSettings,
                      forumSettings,
                     cacheManager,
                     displayDefaultMenuItemSettings
                )
        {
            this._productModelFactory = productModelFactory;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._specificationAttributeService = specificationAttributeService;
            this._productTagService = productTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._topicService = topicService;
            this._eventPublisher = eventPublisher;
            this._searchTermService = searchTermService;
            this._httpContext = httpContext;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._cacheManager = cacheManager;
            this._displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Category model</returns>
        public override CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command)
        {
            var modelFactories = Nop.Core.Infrastructure.EngineContext.Current.ResolveAll<ICatalogModelFactory>();
            var foxModel = modelFactories.Where(x => x.GetType().FullName.Contains("FoxNetSoft.Plugin.Misc.MSSQLProvider")).FirstOrDefault();
            var categtoryModel = new CategoryModel();
            if (foxModel != null)
            {
                categtoryModel = foxModel.PrepareCategoryModel(category, command);

            }
            else
            {
                categtoryModel = base.PrepareCategoryModel(category, command);
            }
            var miscPlugins = _pluginFinder.GetPlugins<CategoryNavigationProvider>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                var result = Helpers.GetPictureUrl(categtoryModel.Id);
                if (_pictureService.GetPictureById(category.PictureId) == null)
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        categtoryModel.PictureModel.ImageUrl = result;
                        categtoryModel.PictureModel.FullSizeImageUrl = result;
                        categtoryModel.PictureModel.ThumbImageUrl = result;
                    }
                }
                foreach (var subCategory in categtoryModel.SubCategories)
                {
                    var subCat = _categoryService.GetCategoryById(subCategory.Id);
                    if (_pictureService.GetPictureById(subCat.PictureId) == null)
                    {
                        result = Helpers.GetPictureUrl(subCategory.Id);
                        if (!string.IsNullOrEmpty(result))
                        {
                            subCategory.PictureModel.ImageUrl = result;
                            subCategory.PictureModel.FullSizeImageUrl = result;
                            subCategory.PictureModel.ThumbImageUrl = result;
                        }
                    }
                }
                foreach (var product in categtoryModel.Products)
                {
                    var picModels = _pictureService.GetPicturesByProductId(product.Id);

                    if (picModels == null || picModels.Count == 0)
                    {
                        result = Helpers.GetPictureUrl(categtoryModel.Id, product.Sku);
                        if (!string.IsNullOrEmpty(result))
                        {
                            product.DefaultPictureModel.ImageUrl = result;
                            product.DefaultPictureModel.FullSizeImageUrl = result;
                            product.DefaultPictureModel.ThumbImageUrl = result;
                        }
                    }
                }
                
            }
            return categtoryModel;
        }

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        public override List<CategoryModel> PrepareHomepageCategoryModels()
        {
            var modelFactories = Nop.Core.Infrastructure.EngineContext.Current.ResolveAll<ICatalogModelFactory>();
            var foxModel = modelFactories.Where(x => x.GetType().FullName.Contains("FoxNetSoft.Plugin.Misc.MSSQLProvider")).FirstOrDefault();
            var categtoryModels = new List<CategoryModel>();
            if (foxModel != null)
            {
                categtoryModels = foxModel.PrepareHomepageCategoryModels();

            }
            else
            {
                categtoryModels = base.PrepareHomepageCategoryModels();
            }
            var miscPlugins = _pluginFinder.GetPlugins<CategoryNavigationProvider>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                foreach (var categtoryModel in categtoryModels)
                {
                    var category = _categoryService.GetCategoryById(categtoryModel.Id);
                    var result = Helpers.GetPictureUrl(categtoryModel.Id);

                    if (_pictureService.GetPictureById(category.PictureId) == null)
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            categtoryModel.PictureModel.ImageUrl = result;
                            categtoryModel.PictureModel.FullSizeImageUrl = result;
                            categtoryModel.PictureModel.ThumbImageUrl = result;
                        }
                    }
                    foreach (var subCategory in categtoryModel.SubCategories)
                    {
                        var subCat = _categoryService.GetCategoryById(subCategory.Id);
                        if (_pictureService.GetPictureById(subCat.PictureId) == null)
                        {
                            result = Helpers.GetPictureUrl(subCategory.Id);
                            if (!string.IsNullOrEmpty(result))
                            {
                                subCategory.PictureModel.ImageUrl = result;
                                subCategory.PictureModel.FullSizeImageUrl = result;
                                subCategory.PictureModel.ThumbImageUrl = result;
                            }
                        }
                    }
                    foreach (var product in categtoryModel.Products)
                    {
                        var picModels = _pictureService.GetPicturesByProductId(product.Id);

                        if (picModels == null || picModels.Count == 0)
                        {
                            result = Helpers.GetPictureUrl(categtoryModel.Id, product.Sku);
                            if (!string.IsNullOrEmpty(result))
                            {
                                product.DefaultPictureModel.ImageUrl = result;
                                product.DefaultPictureModel.FullSizeImageUrl = result;
                                product.DefaultPictureModel.ThumbImageUrl = result;
                            }
                        }
                    }
                    
                }
            }

            return categtoryModels;
        }


    }


}
