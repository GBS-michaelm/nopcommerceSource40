using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using System.Linq;

namespace Nop.Plugin.Catalog.GBS.Components
{
    [ViewComponent(Name = "GBSCategoryNavigation")]
    public class GBSCategoryNavigationComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public GBSCategoryNavigationComponent(IHttpContextAccessor httpContextAccessor,
            ICatalogModelFactoryCustom catalogModelFactoryCustom,
            IStoreContext storeContext,
            ISettingService settingService)
        {
            this._httpContextAccessor = httpContextAccessor;
            _catalogModelFactoryCustom = catalogModelFactoryCustom;
            _storeContext = storeContext;
            this._settingService = settingService;
        }


        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
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
    }
}
