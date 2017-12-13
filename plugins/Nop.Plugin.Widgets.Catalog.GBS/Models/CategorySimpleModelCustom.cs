using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Catalog.GBS.Models
{
    public class CategorySimpleModelCustom : BaseNopEntityModel
    {
        public CategorySimpleModelCustom()
        {
            SubCategories = new List<CategorySimpleModelCustom>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfProducts { get; set; }

        public int ProductsCount { get; set; }

        public bool IncludeInTopMenu { get; set; }        

        public List<CategorySimpleModelCustom> SubCategories { get; set; }
    }
}