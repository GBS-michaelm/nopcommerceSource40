using System;
using System.Collections.Generic;
using Nop.Plugin.BusinessDataAccess.GBS;
using System.Data;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Core.Caching;
using System.Text;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
    public class MarketCenter : CategoryModel
    {

        DBManager manager = new DBManager();
        ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
        ICacheManager cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");


        int _id = 0;
        int _parentCategoryId = 0;
        string _h1 = "";
        string _h2 = "";
        string _topText = "";
        string _bottomText = "";
        string _backgroundImage = "";
        string _foregroundImage = "";
        string _backgroundColor = "";
        string _mainPicturePath;
        string _gatewayHtml = "";
        //List<SubCategoryModel> _subCategories = new List<SubCategoryModel>();
        List<MarketCenter> childCompanies = new List<MarketCenter>();



        //public MarketCenter() { }
        
        public MarketCenter(int marketCenterCategoryId)
        {
            
            this.id = marketCenterCategoryId;
            Category category = categoryService.GetCategoryById(marketCenterCategoryId);
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
            //this.subCategories = (List<SubCategoryModel>)categoryModel.SubCategories;
            this.FeaturedProducts = categoryModel.FeaturedProducts;
            this.Products = categoryModel.Products;
            this.parentCategoryId = _parentCategoryId;


            DataView marketCenterDataView = cacheManager.Get("marketCenter" + marketCenterCategoryId, 60, () => {
                Dictionary<string, Object> marketCenterDic = new Dictionary<string, Object>();
                marketCenterDic.Add("@CategoryId", marketCenterCategoryId);

                string marketCenterDataQuery = "EXEC  @categoryId";
                DataView innerMarketCenterDataView = manager.GetParameterizedDataView(marketCenterDataQuery, marketCenterDic);

                return innerMarketCenterDataView;
            });

            if (marketCenterDataView.Count > 0)
            {
                //team specific data
                this.parentCategoryId = category.ParentCategoryId;
                this.h1 = !string.IsNullOrEmpty(marketCenterDataView[0]["H1"].ToString()) ? marketCenterDataView[0]["H1"].ToString() : this.Name;
                this.h2 = !string.IsNullOrEmpty(marketCenterDataView[0]["H2"].ToString()) ? marketCenterDataView[0]["H2"].ToString() : _h2;
                this.topText = !string.IsNullOrEmpty(marketCenterDataView[0]["UpperText"].ToString()) ? marketCenterDataView[0]["UpperText"].ToString() : _topText;
                this.bottomText = !string.IsNullOrEmpty(marketCenterDataView[0]["LowerText"].ToString()) ? marketCenterDataView[0]["LowerText"].ToString() : _bottomText;
                this.backgroundImage = !string.IsNullOrEmpty(marketCenterDataView[0]["BackgroundPicturePath"].ToString()) ? marketCenterDataView[0]["BackgroundPicturePath"].ToString() : _backgroundImage;
                this.foregroundImage = !string.IsNullOrEmpty(marketCenterDataView[0]["ForegroundPicturePath"].ToString()) ? marketCenterDataView[0]["ForegroundPicturePath"].ToString() : _foregroundImage;
                this.backgroundColor = !string.IsNullOrEmpty(marketCenterDataView[0]["BackgroundColor"].ToString()) ? marketCenterDataView[0]["BackgroundColor"].ToString() : _backgroundColor;
                this.mainPicturePath = !string.IsNullOrEmpty(marketCenterDataView[0]["MainPicturePath"].ToString()) ? marketCenterDataView[0]["MainPicturePath"].ToString() : _mainPicturePath;
                
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
        public string gatewayHtml { get { return _gatewayHtml; } set { _gatewayHtml = value; } }
        //public IList<SubCategoryModel> subCategories { get { return _subCategories; } set { _subCategories = (List<SubCategoryModel>)value; } }



        public Dictionary<string, string> GetMarketCenterHtml()
        {
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            Dictionary<string, string> marketCenterTabsDict = new Dictionary<string, string>();
            List<MarketCenter> alphaList1 = new List<MarketCenter>();
            List<MarketCenter> alphaList2 = new List<MarketCenter>();
            List<MarketCenter> alphaList3 = new List<MarketCenter>();

            //if top level and has children build popup with children links
            IList<Category> topLevelMarketCenters = categoryService.GetAllCategoriesByParentCategoryId(this.id);
            foreach (var marketcenter in topLevelMarketCenters)
            {
                //seperate market centers into alpha lists a-g h-p q-z
                
                

            }

            //call build for featured list


            
            //call build html for each alpha list      
            



            return marketCenterTabsDict;

        }


        private string BuildTabHtml(List<MarketCenter> marketcenterList)
        {
            string tabHtml = "";
            StringBuilder tabHtmlStringBuilder = new StringBuilder();
            //if top lvl and no children. Top level will take user to it's own page

            foreach (var marketCenter in marketcenterList)
            {

            }

            return tabHtml;
        }
        
        private string BuildFeaturedCompanyHtml()
        {
            string featuredHtml = "";
            StringBuilder featuredHtmlStringBuilder = new StringBuilder();
            //if top lvl and no children. Top level will take user to it's own page



            return featuredHtml;
        }


    }

    



}
