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
using Nop.Core.Infrastructure;

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
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;
        private readonly ICacheManager _lifeTimeCacheManager;

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
            IPluginFinder pluginFinder,
            ICatalogModelFactoryCustom catalogModelFactoryCustom) : base(
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
            this._catalogModelFactoryCustom = catalogModelFactoryCustom;
            this._lifeTimeCacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

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
            //var categoryCacheKey = "preparecategorymodelFactory" + category.Id;
            var categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
                category.Id,
                1,
                1,
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                1);
            var finalCategtoryModel = _lifeTimeCacheManager.Get(categoryCacheKey, 1440, () =>
            {

                if (string.IsNullOrEmpty(categtoryModel.PictureModel.ImageUrl))
                {
                    categtoryModel.PictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl();
                    categtoryModel.PictureModel.FullSizeImageUrl = _pictureService.GetDefaultPictureUrl();
                    categtoryModel.PictureModel.ThumbImageUrl = _pictureService.GetDefaultPictureUrl();
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
            });


            return finalCategtoryModel;
        }

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        public override List<CategoryModel> PrepareHomepageCategoryModels()
        {
            var categoryCacheKey = "preparehomepagecategorymodelfactory" + _storeContext.CurrentStore.Id;
            var finalCategtoryModel = _lifeTimeCacheManager.Get(categoryCacheKey, () =>
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
            });

            return finalCategtoryModel;
        }

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Search model</returns>
        public override SearchModel PrepareSearchModel(SearchModel model, CatalogPagingFilteringModel command)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions,
                _catalogSettings.SearchPageProductsPerPage);


            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
               // var allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                var allCategories = _catalogModelFactoryCustom.GetCategoryList(_categoryService, _lifeTimeCacheManager, _storeContext.CurrentStore.Id);

                foreach (var c in allCategories)
                {
                    //generate full category name (breadcrumb)
                    //string categoryBreadcrumb = "";
                    //var breadcrumb = c.GetCategoryBreadCrumb(allCategories, _aclService, _storeMappingService);
                    //for (int i = 0; i <= breadcrumb.Count - 1; i++)
                    //{
                    //    categoryBreadcrumb += breadcrumb[i].GetLocalized(x => x.Name);
                    //    if (i != breadcrumb.Count - 1)
                    //        categoryBreadcrumb += " >> ";
                    //}
                    categoriesModel.Add(new SearchModel.CategoryModel
                    {
                        Id = Int32.Parse(c.Value),
                        Breadcrumb = c.Text
                    });
                }
                return categoriesModel;
            });
            if (categories.Any())
            {
                //first empty entry
                model.AvailableCategories.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                //all other categories
                foreach (var c in categories)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = model.cid == c.Id
                    });
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
            if (manufacturers.Any())
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.GetLocalized(x => x.Name),
                        Selected = model.mid == m.Id
                    });
            }

            model.asv = _vendorSettings.AllowSearchByVendor;
            if (model.asv)
            {
                var vendors = _vendorService.GetAllVendors();
                if (vendors.Any())
                {
                    model.AvailableVendors.Add(new SelectListItem
                    {
                        Value = "0",
                        Text = _localizationService.GetResource("Common.All")
                    });
                    foreach (var vendor in vendors)
                        model.AvailableVendors.Add(new SelectListItem
                        {
                            Value = vendor.Id.ToString(),
                            Text = vendor.GetLocalized(x => x.Name),
                            Selected = model.vid == vendor.Id
                        });
                }
            }

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            var isSearchTermSpecified = false;
            try
            {
                // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
                isSearchTermSpecified = _httpContext.Request.Params["q"] != null;
            }
            catch
            {
                //the "A potentially dangerous Request.QueryString value was detected from the client" exception could be thrown here when some wrong char is specified (e.g. <)
                //although we [ValidateInput(false)] attribute here we try to access "Request.Params" directly
                //that's why we do not re-throw it

                //just ensure that some search term is specified (0 length is not supported inthis case)
                isSearchTermSpecified = !String.IsNullOrEmpty(searchTerms);
            }
            if (isSearchTermSpecified)
            {
                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    int vendorId = 0;
                    if (model.adv)
                    {
                        //advanced search
                        var categoryId = model.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (model.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(GetChildCategoryIds(categoryId));
                            }
                        }

                        manufacturerId = model.mid;

                        //min price
                        if (!string.IsNullOrEmpty(model.pf))
                        {
                            decimal minPrice;
                            if (decimal.TryParse(model.pf, out minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                        }
                        //max price
                        if (!string.IsNullOrEmpty(model.pt))
                        {
                            decimal maxPrice;
                            if (decimal.TryParse(model.pt, out maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                        }

                        if (model.asv)
                            vendorId = model.vid;

                        searchInDescriptions = model.sid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;

                    //products
                    products = _productService.SearchProducts(
                        categoryIds: categoryIds,
                        manufacturerId: manufacturerId,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        priceMin: minPriceConverted,
                        priceMax: maxPriceConverted,
                        keywords: searchTerms,
                        searchDescriptions: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: _workContext.WorkingLanguage.Id,
                        orderBy: (ProductSortingEnum)command.OrderBy,
                        pageIndex: command.PageNumber - 1,
                        pageSize: command.PageSize,
                        vendorId: vendorId);
                    model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();

                    model.NoResults = !model.Products.Any();

                    //search term statistics
                    if (!String.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms, _storeContext.CurrentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = _storeContext.CurrentStore.Id,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new ProductSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = _workContext.WorkingLanguage.Id,
                        VendorId = vendorId
                    });
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);
            return model;
        }

    }


}
