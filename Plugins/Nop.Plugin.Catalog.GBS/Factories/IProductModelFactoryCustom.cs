using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Plugin.Catalog.GBS.Models;
using Nop.Web.Models.Topics;
using Nop.Services.Catalog;
using System.Web.Mvc;
using Nop.Core.Caching;

namespace Nop.Plugin.Catalog.GBS.Factories
{
    public partial interface IProductModelFactoryCustom
    {
        #region methods       


        ProductDetailsModel.ProductPriceModel GetProductPriceModel(Product product);       


        #endregion
    }
}
