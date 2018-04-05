using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Customers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Catalog.GBS.Controllers
{
    public class CustomerRoleController : Nop.Admin.Controllers.CustomerRoleController
    {
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;


        public CustomerRoleController(
            ICustomerService customerService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext,
            ICacheManager cacheManager,
            ICatalogModelFactoryCustom catalogModelFactoryCustom) : base(
                 customerService,
                 localizationService,
                 customerActivityService,
                 permissionService,
                 productService,
                 categoryService,
                 manufacturerService,
                 storeService,
                 vendorService,
                 workContext,
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
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._productService = productService;
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

            this._catalogModelFactoryCustom = catalogModelFactoryCustom;
        }

        public override ActionResult Create()
        {
            ViewResult result = (ViewResult)base.Create();
            return View("~/Administration/Views/CustomerRole/Create.cshtml", result.Model);
            //return base.Create();
        }


        public override ActionResult Edit(int id)
        {
            ViewResult result = (ViewResult)base.Edit(id);
            return View("~/Administration/Views/CustomerRole/Edit.cshtml", result.Model);
            //return base.Edit(id);
        }


        public override ActionResult AssociateProductToCustomerRolePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new CustomerRoleModel.AssociateProductToCustomerRoleModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

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


    }
}