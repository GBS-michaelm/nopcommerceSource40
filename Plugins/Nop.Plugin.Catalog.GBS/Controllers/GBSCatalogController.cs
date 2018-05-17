using Nop.Web.Framework.Controllers;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Web.Factories;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using Nop.Services.Vendors;
using Nop.Services.Common;
using Nop.Services.Security;
using Nop.Services.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Catalog.GBS.Controllers
{
    public class CatalogController : Nop.Web.Controllers.CatalogController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogModelFactory _catalogModelFactory;

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;



        public CatalogController(
            ICatalogModelFactory catalogModelFactory,
            IProductModelFactory productModelFactory,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IProductTagService productTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSetting,
            IStoreService storeService,
            ISettingService settingService) : base(catalogModelFactory,
                     productModelFactory,
                     categoryService,
                     manufacturerService,
                     productService,
                     vendorService,
                     workContext,
                     storeContext,
                     localizationService,
                     webHelper,
                     productTagService,
                     genericAttributeService,
                     aclService,
                     storeMappingService,
                     permissionService,
                     customerActivityService,
                     mediaSettings,
                     catalogSettings,
                     vendorSetting)
        {
            this._categoryService = categoryService;
            this._catalogModelFactory = catalogModelFactory;

            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        [NopHttpsRequirement(SslRequirement.No)]
        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public override ActionResult Category(int categoryId, CatalogPagingFilteringModel command)
        {
            return base.Category(categoryId, command);
        }


        [ChildActionOnly]
        public ActionResult SportsTeamsList(IList<Category> model, int sportsTeamId)
        {
            ViewBag.Id = sportsTeamId;
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult SportsTeamsTest()
        {
            ViewBag.Id = 1;
            return View();
        }
    }
}