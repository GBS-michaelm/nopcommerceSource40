using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Plugin.Catalog.GBS.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using System.Linq;

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

        public WidgetsCategoryNavigationController(ICatalogModelFactoryCustom catalogModelFactoryCustom,                        
            ICategoryService categoryService,
            ICatalogModelFactory catalogModelFactory,
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IStoreContext storeContext)
        {                        
            this._categoryService = categoryService;
            this._catalogModelFactoryCustom = catalogModelFactoryCustom;
            this._catalogModelFactory = catalogModelFactory;

            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
        }
        
        public IActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var categoryNavigationSettings = _settingService.LoadSetting<CategoryNavigationSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AllCategory = categoryNavigationSettings.AllCategory;
            model.NoOfChildren = categoryNavigationSettings.NoOfChildren;
            model.IsActive = categoryNavigationSettings.IsActive;
            model.BlackList = categoryNavigationSettings.BlackList;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllCategory_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.AllCategory, storeScope);
                model.NoOfChildren_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.NoOfChildren, storeScope);
                model.IsActive_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.IsActive, storeScope);
                model.BlackList_OverrideForStore = _settingService.SettingExists(categoryNavigationSettings, x => x.BlackList, storeScope);

            }
            return View("~/Plugins/Catalog.GBS/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
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

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.AllCategory, model.AllCategory_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.NoOfChildren, model.NoOfChildren_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.IsActive, model.IsActive_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(categoryNavigationSettings, x => x.BlackList, model.BlackList_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();         
        }

        [ResponseCache(Duration = 3600, VaryByQueryKeys = new string[] { "*" })]
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
    }
}