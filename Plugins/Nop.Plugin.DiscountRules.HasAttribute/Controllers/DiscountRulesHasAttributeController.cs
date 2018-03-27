using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.HasAttribute.Models;
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

namespace Nop.Plugin.DiscountRules.HasAttribute.Controllers
{
    [AdminAuthorize]
    public class DiscountRulesHasAttributeController : BasePluginController
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

        public DiscountRulesHasAttributeController(IDiscountService discountService,
            ISettingService settingService, 
            IPermissionService permissionService,
            IWorkContext workContext, 
            ILocalizationService localizationService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IStoreService storeService, 
            IVendorService vendorService,
            IProductService productService)
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

            var restrictedAttributeIds = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.RestrictedAttributeIds-{0}", discountRequirementId.HasValue ? discountRequirementId.Value : 0));

            var model = new RequirementModel();
            model.RequirementId = discountRequirementId.HasValue ? discountRequirementId.Value : 0;
            model.DiscountId = discountId;
            model.Attributes = restrictedAttributeIds;

            //add a prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("DiscountRulesHasAttribute{0}", discountRequirementId.HasValue ? discountRequirementId.Value.ToString() : "0");

            return View("~/Plugins/DiscountRules.HasAttribute/Views/DiscountRulesHasAttribute/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Configure(int discountId, int? discountRequirementId, string attributeIds)
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
                _settingService.SetSetting(string.Format("DiscountRequirement.RestrictedAttributeIds-{0}", discountRequirement.Id), attributeIds);
            }
            else
            {
                //save new rule
                discountRequirement = new DiscountRequirement
                {
                    DiscountRequirementRuleSystemName = "DiscountRequirement.HasAttribute"
                };
                discount.DiscountRequirements.Add(discountRequirement);
                _discountService.UpdateDiscount(discount);

                _settingService.SetSetting(string.Format("DiscountRequirement.RestrictedAttributeIds-{0}", discountRequirement.Id), attributeIds);
            }
            return Json(new { Result = true, NewRequirementId = discountRequirement.Id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AttributeAddPopup(string btnId, string attributeIdsInput)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return Content("Access denied");

            var model = new RequirementModel.AddAttributeModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //attributes
            model.AvailableAttributes.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var attributes = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in attributes)
                model.AvailableAttributes.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(attributes), Value = c.Id.ToString() });

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


            ViewBag.attributeIdsInput = attributeIdsInput;
            ViewBag.btnId = btnId;

            return View("~/Plugins/DiscountRules.HasAttribute/Views/DiscountRulesHasAttribute/AttributeAddPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult AttributeAddPopupList(DataSourceRequest command, RequirementModel.AddAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return Content("Access denied");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchAttributeId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );

            var attributes = _categoryService.GetAllCategories(pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);

            var gridModel = new DataSourceResult();
            gridModel.Data = attributes.Select(x => new RequirementModel.AttributeModel
            {
                   Id = x.Id,
                   Name = x.Name,
                   Published = x.Published
            });
            gridModel.Total = attributes.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AdminAntiForgery]
        public ActionResult LoadAttributeFriendlyNames(string attributeIds)
        {
            var result = "";

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return Json(new { Text = result });

            if (!String.IsNullOrWhiteSpace(attributeIds))
            {
                var ids = new List<int>();
                var rangeArray = attributeIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                //we support three ways of specifying Attributes:
                //1. The comma-separated list of Attribute identifiers (e.g. 77, 123, 156).
                //2. The comma-separated list of Attribute identifiers with quantities.
                //      {Attribute ID}:{Quantity}. For example, 77:1, 123:2, 156:3
                //3. The comma-separated list of Attribute identifiers with quantity range.
                //      {Attribute ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
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

                int id1;
                id1 = ids[0];

                //var products = _productService.GetProductsByIds(ids.ToArray());
                //for (int i = 0; i <= products.Count - 1; i++)
                //{
                //    result += products[i].Name;
                //    if (i != products.Count - 1)
                //        result += ", ";
                //}

                var attributes = _categoryService.GetAllCategories();
                var attributes1 = _categoryService.GetAllCategoriesByParentCategoryId(id1);
                for (int i = 0; i <= attributes1.Count - 1; i++)
                {
                    result += attributes1[i].Name;
                    if (i != attributes1.Count - 1)
                        result += ", ";
                }
            }

            return Json(new { Text = result });
        }
    }
}