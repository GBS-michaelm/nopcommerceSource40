using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessDataAccess.GBS;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
    public class GBSProductCategory : CategoryModel
    {

        DBManager manager = new DBManager();

        int _id = 0;
        int _parentCategoryId = 0;
        string _h1 = "";
        string _h2 = "";
        string _topText = "";
        string _bottomText = "";
        string _backgroundImage = "";
        string _foregroundImage = "";
        string _backgroundColor = "";
        string _mainPicturePath = "";

        public GBSProductCategory(int productCategoryId)
        {

            //nop category data
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            this.id = productCategoryId;
            Category category = categoryService.GetCategoryById(productCategoryId);
            ICatalogModelFactory catalogModelFactory = EngineContext.Current.Resolve<ICatalogModelFactory>();
            CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
            catalogPagingFilteringModel.PageSize = 1;
            this.PagingFilteringContext = catalogPagingFilteringModel;
            CategoryModel categoryModel = catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);
            this.Name = categoryModel.Name;
            this.Description = categoryModel.Description;
            this.MetaKeywords = categoryModel.MetaKeywords;
            this.MetaDescription = categoryModel.MetaDescription;
            this.MetaTitle = categoryModel.MetaTitle;
            this.SeName = categoryModel.SeName;
            this.PictureModel = categoryModel.PictureModel;
            this.PagingFilteringContext = categoryModel.PagingFilteringContext;
            this.DisplayCategoryBreadcrumb = categoryModel.DisplayCategoryBreadcrumb;
            this.CategoryBreadcrumb = categoryModel.CategoryBreadcrumb;
            this.SubCategories = categoryModel.SubCategories;
            this.FeaturedProducts = categoryModel.FeaturedProducts;
            this.Products = categoryModel.Products;

            //datalook up via category(product category) id from gbscategory table
            Dictionary<string, Object> productCategoryDic = new Dictionary<string, Object>();
            productCategoryDic.Add("@CategoryId", productCategoryId);

            string productCategoryDataQuery = "EXEC usp_SelectGBSCustomCategoryData @CategoryId";
            DataView productCategoryDataView = manager.GetParameterizedDataView(productCategoryDataQuery, productCategoryDic);

            if (productCategoryDataView.Count > 0)
            {
                //product category specific data
                this.parentCategoryId = category.ParentCategoryId;
                this.h1 = !string.IsNullOrEmpty(productCategoryDataView[0]["H1"].ToString()) ? productCategoryDataView[0]["H1"].ToString() : this.Name;
                this.h2 = !string.IsNullOrEmpty(productCategoryDataView[0]["H2"].ToString()) ? productCategoryDataView[0]["H2"].ToString() : _h2;
                this.topText = !string.IsNullOrEmpty(productCategoryDataView[0]["UpperText"].ToString()) ? productCategoryDataView[0]["UpperText"].ToString() : _topText;
                this.bottomText = !string.IsNullOrEmpty(productCategoryDataView[0]["LowerText"].ToString()) ? productCategoryDataView[0]["LowerText"].ToString() : _bottomText;
                this.backgroundImage = !string.IsNullOrEmpty(productCategoryDataView[0]["BackgroundPicturePath"].ToString()) ? productCategoryDataView[0]["BackgroundPicturePath"].ToString() : _backgroundImage;
                this.foregroundImage = !string.IsNullOrEmpty(productCategoryDataView[0]["ForegroundPicturePath"].ToString()) ? productCategoryDataView[0]["ForegroundPicturePath"].ToString() : _foregroundImage;
                this.backgroundColor = !string.IsNullOrEmpty(productCategoryDataView[0]["BackgroundColor"].ToString()) ? productCategoryDataView[0]["BackgroundColor"].ToString() : _backgroundColor;
                this.mainPicturePath = !string.IsNullOrEmpty(productCategoryDataView[0]["MainPicturePath"].ToString()) ? productCategoryDataView[0]["MainPicturePath"].ToString() : _mainPicturePath;

            }

        }

        public int id { get { return _id; } set { _id = value; } }
        public int parentCategoryId { get { return _parentCategoryId; } set { _parentCategoryId = value; } }

        public string h1 { get { return _h1; } set { _h1 = value; } }
        public string h2 { get { return _h2; } set { _h2 = value; } }
        public string topText { get { return _topText; } set { _topText = value; } }
        public string bottomText { get { return _bottomText; } set { _bottomText = value; } }
        public string backgroundImage { get { return _backgroundImage; } set { _backgroundImage = value; } }
        public string foregroundImage { get { return _foregroundImage; } set { _foregroundImage = value; } }
        public string backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public string mainPicturePath { get { return _mainPicturePath; } set { _mainPicturePath = value; } }

    }

}
