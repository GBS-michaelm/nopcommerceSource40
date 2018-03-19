using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Plugin.Catalog.GBS.Models;
using Nop.Web.Models.Topics;

namespace Nop.Plugin.Catalog.GBS.Factories
{
    public partial interface ICatalogModelFactoryCustom
    {        
        #region Categories       

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        CategoryNavigationModelCustom PrepareCategoryNavigationModel(int currentCategoryId,
            int currentProductId);       

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModelCustom> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModelCustom> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null);

        /// <summary>
        /// Prepare category Topic model.  Grabs Topic data associated with a category for use with Tab display on Category Pages
        /// </summary>
        /// <param name="catId">Category Id</param>
        /// <returns>TopicModel</returns>
        TopicModel PrepareCategoryTabTopicModel(int catId);

        /// <summary>
        /// Prepare category featured product detail model.  Grabs product data associated with featured products
        /// </summary>
        /// <param name="catId">Category Id</param>
        /// <returns>TopicModel</returns>
        ProductDetailsModel PrepareCategoryFeaturedProductDetailsModel(int catId);


        #endregion
    }
}
