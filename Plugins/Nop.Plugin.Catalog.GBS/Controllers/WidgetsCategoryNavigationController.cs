using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Catalog.GBS.Models;
using System.Linq;
using Nop.Services.Security;
using System.Collections.Generic;
using Nop.Services.Catalog;
using System;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Plugin.Catalog.GBS.DataAccess;
using System.Data;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Themes;
using Nop.Services.Topics;
using Nop.Core.Domain.Topics;
using Nop.Web.Models.Catalog;
using Nop.Services.Logging;

namespace Nop.Plugin.Catalog.GBS.Controllers
{
    public class WidgetsCategoryNavigationController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;
        private readonly ICatalogModelFactory _catalogModelFactory;

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        public readonly IStoreContext _storeContext;
        public readonly ITopicTemplateService _topicTemplateService;
        private readonly ILogger _logger;


        public WidgetsCategoryNavigationController(ICatalogModelFactoryCustom catalogModelFactoryCustom,
            ICategoryService categoryService,
            ICatalogModelFactory catalogModelFactory,
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ITopicTemplateService topicTemplateService,
            ILogger logger
            )
        {
            this._categoryService = categoryService;
            this._catalogModelFactoryCustom = catalogModelFactoryCustom;
            this._catalogModelFactory = catalogModelFactory;

            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._topicTemplateService = topicTemplateService;
            this._logger = logger;

        }


        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var categoryNavigationSettings = _settingService.LoadSetting<CategoryNavigationSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AllCategory = categoryNavigationSettings.AllCategory;
            model.NoOfChildren = categoryNavigationSettings.NoOfChildren;
            model.IsActive = categoryNavigationSettings.IsActive;
            model.BlackList = categoryNavigationSettings.BlackList;
            model.CategoryTabsSpecAttrName = categoryNavigationSettings.CategoryTabsSpecAttrName;
            

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllCategory_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.AllCategory, storeScope);
                model.NoOfChildren_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.NoOfChildren, storeScope);
                model.IsActive_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.IsActive, storeScope);
                model.BlackList_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.BlackList, storeScope);
                model.CategoryTabsSpecAttrName_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.CategoryTabsSpecAttrName, storeScope);

            }
            return View("~/Plugins/Catalog.GBS/Views/Configure.cshtml", model);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var categoryNavigationSettings = _settingService.LoadSetting<CategoryNavigationSettings>(storeScope);

            //save settings
            categoryNavigationSettings.AllCategory = model.AllCategory;
            categoryNavigationSettings.NoOfChildren = model.NoOfChildren;
            categoryNavigationSettings.IsActive = model.IsActive;
            categoryNavigationSettings.BlackList = model.BlackList;
            categoryNavigationSettings.CategoryTabsSpecAttrName = model.CategoryTabsSpecAttrName;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.AllCategory, model.AllCategory_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.NoOfChildren, model.NoOfChildren_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.IsActive, model.IsActive_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.BlackList, model.BlackList_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.CategoryTabsSpecAttrName, model.CategoryTabsSpecAttrName_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult CategoryNavigation(string widgetZone, object additionalData = null)
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.CurrentStore.Id;
            var categoryNavigationSettings = _settingService.LoadSetting<CategoryNavigationSettings>(storeScope);
            int currentCategoryId = 0;
            int currentProductId = 0;
            if (additionalData != null)
            {
                string[] data = additionalData.ToString().Split(',');
                if (data.Any())
                {
                    int.TryParse(data[0], out currentCategoryId);
                    if (data.Count() > 1)
                    {
                        int.TryParse(data[1], out currentProductId);
                    }
                }
            }
            //if (categoryNavigationSettings.IsActive)
            //{
            var model = _catalogModelFactoryCustom.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
            return View("~/Plugins/Catalog.GBS/Views/CategoryNavigationCustom.cshtml", model);
            //}
            //else
            //{
            //    var model = _catalogModelFactory.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
            //    return View("~/Plugins/Catalog.GBS/Views/CategoryNavigation.cshtml", model);
            //}
        }

        public static bool HasSubcategoryProducts(CategorySimpleModelCustom category)
        {
            if (category.SubCategories.Any())
            {
                foreach (var subcategory in category.SubCategories)
                {
                    if (subcategory.ProductsCount > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return HasSubcategoryProducts(subcategory);
                    }
                }
            }
            return false;
        }

        [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult CategoryTabs(string widgetZone, object additionalData = null)
        {
            var currentCategoryId = 0;
            try
            {

                if (additionalData != null)
                {
                    int.TryParse(additionalData.ToString(), out currentCategoryId);
                }
                //get topic id from mapping table
                var model = _catalogModelFactoryCustom.PrepareCategoryTabTopicModel(currentCategoryId);
                if (model == null) {
                    _logger.Error("Model is null in CategoryTabs: category id = " +currentCategoryId);
                    return null;
                }
                //substitute viewpath with topic template.
                //do this dynamically so that different templates can be used for this widget.
                var themeName = EngineContext.Current.Resolve<IThemeContext>().WorkingThemeName;
                TopicTemplate topicTemplate = _topicTemplateService.GetTopicTemplateById(model.TopicTemplateId);
                if (topicTemplate == null) {
                    _logger.Error("topicTemplate is null in CategoryTabs: category id = " + currentCategoryId);
                    return null;
                }

                //get pricing
                ViewBag.ProductDetailModel = _catalogModelFactoryCustom.PrepareCategoryFeaturedProductDetailsModel(currentCategoryId);

                return View("~/Themes/" + themeName + "/Views/Topic/" + topicTemplate.ViewPath + ".cshtml", model);
            } catch (Exception ex)
            {
                _logger.Error("Exception in CategoryTabs: category id = "+currentCategoryId, ex);

                return null;
            }

        }


    }
}