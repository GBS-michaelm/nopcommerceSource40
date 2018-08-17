using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Catalog.GBS.Components
{
    [ViewComponent(Name = "CategoryNavigationPlugin")]
    public class CategoryNavigationPluginComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;
        private readonly IStoreContext _storeContext;

        public CategoryNavigationPluginComponent(IHttpContextAccessor httpContextAccessor,
            ICatalogModelFactoryCustom catalogModelFactoryCustom,
            IStoreContext storeContext)
        {
            this._httpContextAccessor = httpContextAccessor;
            _catalogModelFactoryCustom = catalogModelFactoryCustom;
            _storeContext = storeContext;
        }


        public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
        {   
            var model = _catalogModelFactoryCustom.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
            return View("~/Plugins/Catalog.GBS/Views/CategoryNavigationCustom.cshtml", model);
        }
    }
}
