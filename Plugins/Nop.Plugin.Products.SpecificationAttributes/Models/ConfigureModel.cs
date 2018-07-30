using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Plugin.Products.SpecificationAttributes.Models
{
	public class ConfigureModel : BaseNopEntityModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugin.ProductArtist.ArtistName")]
        public string ArtistName { get; set; }

        [NopResourceDisplayName("Plugin.SpecificationAttributes.ArtistCategoryID")]
        public int ArtistCategoryID { get; set; }
        public bool ArtistCategoryID_OverrideForStore { get; set; }
    }

    public class ProductCategoryTitleModel : BaseNopEntityModel
    {
        public string CategoryName { get; set; }

        public string SeName { get; set; }
    }

    public class ProductByArtistModel: BaseNopEntityModel
    {
        public ProductByArtistModel()
        {
            Products = new List<ProductOverviewModel>();
        }

        public string ArtistName { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }
    }
}
