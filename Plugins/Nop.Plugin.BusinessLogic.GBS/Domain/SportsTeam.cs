﻿using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessDataAccess.GBS;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{

    public class SportsTeam : CategoryModel
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
        int _featuredProductId = 2222;
        bool _isFeatured = false;
        string _gatewayHtml = "";

        public SportsTeam(int teamId)
        {

            //nop category data
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            this.id = teamId;
            Category category = categoryService.GetCategoryById(teamId);
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

            //datalook up via category(sports team) id from gbscategory table
            //Dictionary<string, Object> sportsTeamDic = new Dictionary<string, Object>();
            //sportsTeamDic.Add("@CategoryId", teamId);

            //string sportsTeamDataQuery = "EXEC usp_SelectGBSCustomSportsTeam @categoryId";
            //DataView sportsTeamDataView = manager.GetParameterizedDataView(sportsTeamDataQuery, sportsTeamDic);

            ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            DataView sportsTeamDataView = cacheManager.Get("sportsTeam" + teamId, 60, () => {
                Dictionary<string, Object> sportsTeamDic = new Dictionary<string, Object>();
                sportsTeamDic.Add("@CategoryId", teamId);

                string sportsTeamDataQuery = "EXEC usp_SelectGBSCustomSportsTeam @categoryId";
                DataView innerSportsTeamDataView = manager.GetParameterizedDataView(sportsTeamDataQuery, sportsTeamDic);

                return innerSportsTeamDataView;
            });

            if (sportsTeamDataView.Count > 0)
            {
                //team specific data
                this.parentCategoryId = category.ParentCategoryId;
                this.h1 = !string.IsNullOrEmpty(sportsTeamDataView[0]["H1"].ToString()) ? sportsTeamDataView[0]["H1"].ToString() : this.Name;
                this.h2 = !string.IsNullOrEmpty(sportsTeamDataView[0]["H2"].ToString()) ? sportsTeamDataView[0]["H2"].ToString() : _h2;
                this.topText = !string.IsNullOrEmpty(sportsTeamDataView[0]["UpperText"].ToString()) ? sportsTeamDataView[0]["UpperText"].ToString() : _topText;
                this.bottomText = !string.IsNullOrEmpty(sportsTeamDataView[0]["LowerText"].ToString()) ? sportsTeamDataView[0]["LowerText"].ToString() : _bottomText;
                this.backgroundImage = !string.IsNullOrEmpty(sportsTeamDataView[0]["BackgroundPicturePath"].ToString()) ? sportsTeamDataView[0]["BackgroundPicturePath"].ToString() : _backgroundImage;
                this.foregroundImage = !string.IsNullOrEmpty(sportsTeamDataView[0]["ForegroundPicturePath"].ToString()) ? sportsTeamDataView[0]["ForegroundPicturePath"].ToString() : _foregroundImage;
                this.backgroundColor = !string.IsNullOrEmpty(sportsTeamDataView[0]["BackgroundColor"].ToString()) ? sportsTeamDataView[0]["BackgroundColor"].ToString() : _backgroundColor;
                this.mainPicturePath = !string.IsNullOrEmpty(sportsTeamDataView[0]["MainPicturePath"].ToString()) ? sportsTeamDataView[0]["MainPicturePath"].ToString() : _mainPicturePath;
                int featuredId;
                this.featuredProductId = Int32.TryParse(sportsTeamDataView[0]["FeaturedProductId"].ToString(), out featuredId) ? featuredId : _featuredProductId;
                this.isFeatured = sportsTeamDataView[0]["IsFeatured"] != null && sportsTeamDataView[0]["IsFeatured"] != DBNull.Value ? (Convert.ToBoolean(sportsTeamDataView[0]["IsFeatured"]) == true ? true : false) : _isFeatured; 
            }

        }

        public int id { get { return _id; } set { _id = value; } }
        public int parentCategoryId { get { return _parentCategoryId; } set { _parentCategoryId = value; } }
        public string h1 { get { return _h1; } set { _h1 = value; } }
        public string h2 { get { return _h2; } set { _h2 = value; } }
        public string topText { get{ return _topText; } set{ _topText = value; } }
        public string bottomText { get{ return _bottomText; } set{ _bottomText = value; } }
        public string backgroundImage { get{ return _backgroundImage; } set{ _backgroundImage = value; } }
        public string foregroundImage { get{ return _foregroundImage; } set{ _foregroundImage = value; } }
        public string backgroundColor { get{ return _backgroundColor; } set{ _backgroundColor = value; } }
        public string mainPicturePath { get { return _mainPicturePath; } set { _mainPicturePath = value; } }
        public int featuredProductId { get { return _featuredProductId; } set { _featuredProductId = value; } }
        public bool isFeatured { get { return _isFeatured; } set { _isFeatured = value; } }
        public string gatewayHtml { get { return _gatewayHtml; } set { _gatewayHtml = value; } }


        public List<SportsTeam> CreateSportsTeamListFromCategories(IList<Category> teamList)
        {
            List<SportsTeam> teams = new List<SportsTeam>();

            ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            if (teamList.Count > 0)
            {
                teams = cacheManager.Get("sportsTeamList" + teamList[0].ParentCategoryId, 60, () => {

                    foreach (var team in teamList)
                    {

                        SportsTeam curTeam = new SportsTeam(team.Id);
                        teams.Add(curTeam);
                    }

                    return teams;

                });
            }   
            
            return teams;
        }

        public DataView GetExtendedTeamData(int teamId)
        {
            Dictionary<string, Object> sportsTeamDic = new Dictionary<string, Object>();
            sportsTeamDic.Add("@CategoryId", teamId);

            string sportsTeamDataQuery = "EXEC usp_SelectGBSCustomSportsTeam @categoryId";
            DataView sportsTeamDataView = manager.GetParameterizedDataView(sportsTeamDataQuery, sportsTeamDic);

            return sportsTeamDataView;
        }

        public string GenerateTeamProductHtml()
        {
            StringBuilder teamHtmlStringBuilder = new StringBuilder();
            var iCategoryService = EngineContext.Current.Resolve<ICategoryService>();
            IList<Category> teamCategoryProducts = iCategoryService.GetAllCategoriesByParentCategoryId(this.id);
            //var orderedTeamCategoryProducts = teamCategoryProducts.OrderBy(x => x.DisplayOrder).ToList();

            var iProductService = EngineContext.Current.Resolve<IProductService>();


            teamHtmlStringBuilder.Append("<div class='pricing bottommargin clearfix'>");

            foreach (var category in teamCategoryProducts)
            {

                SportsTeam customExtendedCategoryData = new SportsTeam(category.Id);
                IPagedList<ProductCategory> productCategoryList = iCategoryService.GetProductCategoriesByCategoryId(category.Id);
                Product featuredProductData = iProductService.GetProductById(customExtendedCategoryData.featuredProductId);



                teamHtmlStringBuilder.Append("<div class='col-md-3'>");
                if(customExtendedCategoryData.isFeatured == true)
                {
                    teamHtmlStringBuilder.Append("    <div class='pricing-box best-price'>");
                }
                else
                {
                    teamHtmlStringBuilder.Append("    <div class='pricing-box'>");
                }               
                teamHtmlStringBuilder.Append("        <div class='pricing-title'>");
                teamHtmlStringBuilder.Append("            <h3>" + category.Name + "</h3>");
                teamHtmlStringBuilder.Append("        </div>");
                teamHtmlStringBuilder.Append("        <div class='gateway-image'><img src='"+ customExtendedCategoryData.mainPicturePath + "'/></div>");
                teamHtmlStringBuilder.Append("        <div class='pricing-features'>");
                teamHtmlStringBuilder.Append("            <ul>");
                teamHtmlStringBuilder.Append("                <li>" + featuredProductData.Width.ToString("f1") + "\" X " + featuredProductData.Length.ToString("f1") + "\"</li> ");
                teamHtmlStringBuilder.Append("                <li>" + category.Name + " top</li>");
                teamHtmlStringBuilder.Append("                <li></li>"); //spec attributes stuff
                teamHtmlStringBuilder.Append("                <li>" + productCategoryList.Count + " designs available</li>");
                teamHtmlStringBuilder.Append("            </ul>");
                teamHtmlStringBuilder.Append("            <div class='gateway-pricing'>Starting at $"+ (int)featuredProductData.Price + "</div>");
                teamHtmlStringBuilder.Append("        </div>"); //ask aboud naveeds image front/back handler
                if(customExtendedCategoryData.isFeatured == true)
                {
                    teamHtmlStringBuilder.Append("        <div class='pricing-action'><a href='' class='team-handle button button-rounded button-reveal button-large button-green tright'><i class='icon-angle-right'></i><span>Select</span></a></div>");
                }
                else
                {
                    teamHtmlStringBuilder.Append("        <div class='pricing-action'><a href='' class='team-handle button button-rounded button-reveal button-large button-blue tright'><i class='icon-angle-right'></i><span>Select</span></a></div>");

                }
                teamHtmlStringBuilder.Append("    </div>");
                teamHtmlStringBuilder.Append("</div>");

            }

            teamHtmlStringBuilder.Append("</div>");

            return teamHtmlStringBuilder.ToString();
        }

    }



}
