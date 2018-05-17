using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Infrastructure.Cache;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Factories;
using Nop.Admin.Controllers;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Catalog.GBS.Controllers
{
    public class CategoryController : Nop.Admin.Controllers.CategoryController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogModelFactory _catalogModelFactory;

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly VendorSettings _vendorSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IManufacturerService _manufacturerService;
        private readonly IShippingService _shippingService;
        private readonly IVendorService _vendorService;
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;


        public CategoryController(
            ICategoryService categoryService, 
            ICategoryTemplateService categoryTemplateService,
            IManufacturerService manufacturerService, 
            IProductService productService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IDiscountService discountService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            IImportManager importManager,
            ICacheManager cacheManager,
            ICatalogModelFactoryCustom catalogModelFactoryCustom) : base(
                     categoryService, 
                     categoryTemplateService,
                     manufacturerService, 
                     productService,
                     customerService,
                     urlRecordService,
                     pictureService,
                     languageService,
                     localizationService,
                     localizedEntityService,
                     discountService,
                     permissionService,
                     aclService,
                     storeService,
                     storeMappingService,
                     exportManager,
                     vendorService,
                     customerActivityService,
                     catalogSettings,
                     workContext,
                     importManager,
                     cacheManager
                )
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

            this._catalogModelFactoryCustom = catalogModelFactoryCustom;
        }

        public override ActionResult Edit(int id)
        {
            ViewResult result = (ViewResult)base.Edit(id);
            return View("~/Administration/Views/Category/Edit.cshtml", result.Model);
        }

        public override ActionResult Create()
        {
            ViewResult result = (ViewResult)base.Create();
            return View("~/Administration/Views/Category/Create.cshtml", result.Model);
        }

        public override ActionResult ProductAddPopup(int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new CategoryModel.AddCategoryProductModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            var categories = _catalogModelFactoryCustom.GetCategoryList(_categoryService, _cacheManager);

            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [NonAction]
        protected override void PrepareAllCategoriesModel(CategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCategories.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"),
                Value = "0"
            });
            //var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            var categories = _catalogModelFactoryCustom.GetCategoryList(_categoryService, _cacheManager);

            foreach (var c in categories)
                model.AvailableCategories.Add(c);
        }

    }
}