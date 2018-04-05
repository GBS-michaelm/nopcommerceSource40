using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.DiscountRules.HasCategory.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.HasCategory.Fields.Categories")]
        public string Categories { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        #region Nested classes

        public partial class AddCategoryModel : BaseNopModel
        {
            public AddCategoryModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class CategoryModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool Published { get; set; }
        }
        #endregion
    }
}