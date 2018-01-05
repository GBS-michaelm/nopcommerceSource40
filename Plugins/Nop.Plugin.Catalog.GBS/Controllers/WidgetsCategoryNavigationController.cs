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

        public WidgetsCategoryNavigationController(ICatalogModelFactoryCustom catalogModelFactoryCustom,                        
            ICategoryService categoryService,
            ICatalogModelFactory catalogModelFactory,
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService)
        {                        
            this._categoryService = categoryService;
            this._catalogModelFactoryCustom = catalogModelFactoryCustom;
            this._catalogModelFactory = catalogModelFactory;

            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
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

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllCategory_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.AllCategory, storeScope);
                model.NoOfChildren_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.NoOfChildren, storeScope);
                model.IsActive_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.IsActive, storeScope);
            }
            return View("~/Plugins/Nop.Plugin.Catalog.GBS/Views/Configure.cshtml", model);
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

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.AllCategory, model.AllCategory_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.NoOfChildren, model.NoOfChildren_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.IsActive, model.IsActive_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();         
        }

        [ChildActionOnly]
        public ActionResult CategoryNavigation(string widgetZone, object additionalData = null)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
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
            if (categoryNavigationSettings.IsActive)
            {
                var model = _catalogModelFactoryCustom.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);              
                return View("~/Plugins/Nop.Plugin.Catalog.GBS/Views/CategoryNavigationCustom.cshtml", model);
            }
            else
            {
                var model = _catalogModelFactory.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
                return View("~/Plugins/Nop.Plugin.Catalog.GBS/Views/CategoryNavigation.cshtml", model);
            }
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
    }
}