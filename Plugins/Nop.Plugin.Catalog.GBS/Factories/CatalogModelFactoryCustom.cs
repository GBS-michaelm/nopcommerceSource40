﻿using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Catalog.GBS.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Catalog.GBS.Factories
{
    public partial class CatalogModelFactoryCustom : ICatalogModelFactoryCustom
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;

        private readonly CategoryNavigationSettings _categoryNavigationSettings;
        private readonly ILogger _logger;


        #endregion

        #region Constructors

        public CatalogModelFactoryCustom(
            ILogger logger,
            ICategoryService categoryService,
            IProductService productService,
            IWorkContext workContext,
            IStoreContext storeContext,
            CatalogSettings catalogSettings,
            ICacheManager cacheManager,
            CategoryNavigationSettings categoryNavigationSettings)
        {
            this._logger = logger;
            this._categoryService = categoryService;
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._catalogSettings = catalogSettings;
            this._cacheManager = cacheManager;

            this._categoryNavigationSettings = categoryNavigationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get child category identifiers
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <returns>List of child category identifiers</returns>
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        public virtual CategoryNavigationModelCustom PrepareCategoryNavigationModel(int currentCategoryId, int currentProductId)
        {
            //get active category
            int activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentProductId > 0)
            {
                //product details page
                var productCategories = _categoryService.GetProductCategoriesByProductId(currentProductId);
                if (productCategories.Any())
                    activeCategoryId = productCategories[0].CategoryId;
            }

            var cachedCategoriesModel = PrepareCategorySimpleModels();
            var model = new CategoryNavigationModelCustom
            {
                CurrentCategoryId = activeCategoryId,
                Categories = cachedCategoriesModel,
                AllCategory = _categoryNavigationSettings.AllCategory,
                NoOfChildren = _categoryNavigationSettings.NoOfChildren
            };
            return model;
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        public virtual List<CategorySimpleModelCustom> PrepareCategorySimpleModels()
        {
            //load and cache them
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PrepareCategorySimpleModels(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of category (simple) models</returns>
        public virtual List<CategorySimpleModelCustom> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModelCustom>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemeneted it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once
                allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            List<int> BlackList = new List<int>();
            try
            {
                if (!string.IsNullOrEmpty(_categoryNavigationSettings.BlackList))
                {
                    BlackList = _categoryNavigationSettings.BlackList.Split(',').Select(int.Parse).ToList();

                }
            }
            catch (Exception ex)
            {
                _logger.Error("BlackList misconfigured in Catalog Plugin - ", ex);
            }
            foreach (var category in categories)
            {
                if (BlackList.Contains(category.Id))
                {
                    continue;
                }
                var cats = new List<int>();
                cats.Add(category.Id);

                var categoryModel = new CategorySimpleModelCustom
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu,
                    ProductsCount = _productService.GetNumberOfProductsInCategory(cats, _storeContext.CurrentStore.Id)
                };


                //number of products in each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id,
                        category.Id);
                    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                    {
                        var categoryIds = new List<int>();
                        categoryIds.Add(category.Id);
                        //include subcategories
                        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                            categoryIds.AddRange(GetChildCategoryIds(category.Id));
                        return _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                    });
                }

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    if (subCategories.Count > 0)
                    {
                        categoryModel.SubCategories.AddRange(subCategories);
                    }
                }
                if (categoryModel.ProductsCount > 0 || categoryModel.SubCategories.Count > 0)
                {
                    result.Add(categoryModel);
                }
            }

            return result;
        }

        #endregion
    }
}
