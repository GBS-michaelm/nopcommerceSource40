using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.HasCategory.Models;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Security;
using Nop.Core.Caching;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.DiscountRules.HasCategory.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesHasCategoryController : BasePluginController
    {
        private readonly IDiscountService _discountService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IProductService _productService;
        private readonly ICacheManager _cacheManager;
        private readonly ICatalogModelFactoryCustom _catalogModelFactoryCustom;


        public DiscountRulesHasCategoryController(IDiscountService discountService,
            ISettingService settingService, 
            IPermissionService permissionService,
            IWorkContext workContext, 
            ILocalizationService localizationService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IStoreService storeService, 
            IVendorService vendorService,
            IProductService productService,
            ICacheManager cacheManager,
            ICatalogModelFactoryCustom catalogModelFactoryCustom)
        {
            this._discountService = discountService;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._productService = productService;
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static"); 
            this._catalogModelFactoryCustom = catalogModelFactoryCustom;

        }

        public ActionResult Configure(int discountId, int? discountRequirementId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            if (discountRequirementId.HasValue)
            {
                var discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId.Value);
                if (discountRequirement == null)
                    return Content("Failed to load requirement.");
            }

            var restrictedCategoryIds = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.RestrictedCategoryIds-{0}", discountRequirementId.HasValue ? discountRequirementId.Value : 0));

            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.Categories = restrictedCategoryIds;

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesHasCategory{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("~/Plugins/DiscountRules.HasCategory/Views/DiscountRulesHasCategory/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Configure(int discountId, int? discountRequirementId, string categoryIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            DiscountRequirement discountRequirement = null;
            if (discountRequirementId.HasValue)
                discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId.Value);

            if (discountRequirement != null)
            {
                //update existing rule
                _settingService.SetSetting(string.Format("DiscountRequirement.RestrictedCategoryIds-{0}", discountRequirement.Id), categoryIds);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement
                {
                    DiscountRequirementRuleSystemName = "DiscountRequirement.HasCategory"
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);

                _settingService.SetSetting(string.Format("DiscountRequirement.RestrictedCategoryIds-{0}", discountRequirement.Id), categoryIds);
            }
            return Json(new { Result = true, NewRequirementId = discountRequirement.Id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CategoryAddPopup(string btnId, string categoryIdsInput)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return Content("Access denied");

            var model = new RequirementModel.AddCategoryModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //var categories = _categoryService.GetAllCategories(showHidden: true);
            //foreach (var c in categories)
            //    model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });
            var categories = _catalogModelFactoryCustom.GetCategoryList(_categoryService, _cacheManager);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });


            ViewBag.categoryIdsInput = categoryIdsInput;
            ViewBag.btnId = btnId;

            return View("~/Plugins/DiscountRules.HasCategory/Views/DiscountRulesHasCategory/CategoryAddPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult CategoryAddPopupList(DataSourceRequest command, RequirementModel.AddCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return Content("Access denied");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            ////var allcategories = _categoryService.GetAllCategories(pageIndex: command.Page - 1,
            //        pageSize: command.PageSize,
            //        showHidden: true);


            var categories = new PagedList<Category>(new List<Category>(), command.Page - 1, command.PageSize);

            if (model.SearchCategoryId == 0)
            {
                categories = (PagedList<Category>)_categoryService.GetAllCategories(pageIndex: command.Page - 1,
                    pageSize: command.PageSize,
                    showHidden: true);
            }
            else
            {
                var categories1 = _categoryService.GetAllCategoriesByParentCategoryId(model.SearchCategoryId, false, true);
                var sortedList = (List<Category>)categories1;
                sortedList.Sort(CompareCategoryByName);
                categories1 = sortedList;

                var categoriesList = new PagedList<Category>(new List<Category>(), command.Page - 1, command.PageSize);

                foreach (Category category in categories1)
                {

                    if (isADescendant(category, model.SearchCategoryId))
                    {
                        categoriesList.Add(category);
                    }
                }

                categories = categoriesList;
            }


            //var xx = (PagedList<Category>)categories;
            //xx.Sort(CompareCategoryByName);
            //categories = xx;

            var gridModel = new DataSourceResult();
            gridModel.Data = categories.Select(x => new RequirementModel.CategoryModel
            {
                   Id = x.Id,
                   Name = x.GetFormattedBreadCrumb(_categoryService),
                   Published = x.Published
            });

            gridModel.Total = categories.Count;

            return Json(gridModel);
        }

        private bool isADescendant(Category category, int parentId)
        {
            if (category.ParentCategoryId == 0)
            {
                return false;
            }

            if (category.ParentCategoryId == parentId)
            {
                return true;
            }
            else
            {
                return isADescendant(_categoryService.GetCategoryById(category.ParentCategoryId), parentId);
            }
        }

        public static int CompareCategoryByName(Category cat1, Category cat2)
        {
            return String.Compare(cat1.Name, cat2.Name);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AdminAntiForgery]
        public ActionResult LoadCategoryFriendlyNames(string categoryIds)
        {
            var result = "";

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return Json(new { Text = result });

            if (!String.IsNullOrWhiteSpace(categoryIds))
            {
                var ids = new List<int>();
                var rangeArray = categoryIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                //we support three ways of specifying Categories:
                //1. The comma-separated list of Category identifiers (e.g. 77, 123, 156).
                //2. The comma-separated list of Category identifiers with quantities.
                //      {Category ID}:{Quantity}. For example, 77:1, 123:2, 156:3
                //3. The comma-separated list of Category identifiers with quantity range.
                //      {Category ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
                foreach (string str1 in rangeArray)
                {
                    var str2 = str1;
                    //we do not display specified quantities and ranges
                    //so let's parse only product names (before : sign)
                    if (str2.Contains(":"))
                        str2 = str2.Substring(0, str2.IndexOf(":"));

                    int tmp1;
                    if (int.TryParse(str2, out tmp1))
                        ids.Add(tmp1);
                }

                for (int i = 0; i <= ids.Count - 1; i++)
                {
                    result += _categoryService.GetCategoryById(ids[i]).Name;
                    if (i != ids.Count - 1)
                        result += ", ";
                }

            }

            return Json(new { Text = result });
        }
    }
}