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
        bool _isTopCompany = false; //featuredCompany ??
        List<MarketCenter> _childCompanies = new List<MarketCenter>();
                        
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
            this.SubCategories = categoryModel.SubCategories;
            this.FeaturedProducts = categoryModel.FeaturedProducts;
            this.Products = categoryModel.Products;
            this.parentCategoryId = _parentCategoryId;
            IPictureService pictureService = EngineContext.Current.Resolve<IPictureService>();
            this.mainPicturePath = pictureService.GetPictureUrl(category.PictureId);


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
                this.isTopCompany = marketCenterDataView[0]["IsFeatured"] != null ? (bool)marketCenterDataView[0]["IsFeatured"] : isTopCompany = _isTopCompany;
            }

            #region child companies

            //child companies handling
            DataView marketCenterChildCompanyDataView = cacheManager.Get("marketCenterChildCompany" + marketCenterCategoryId, 60, () => {
                Dictionary<string, Object> marketCenterChildCompanyDic = new Dictionary<string, Object>();
                marketCenterChildCompanyDic.Add("@CategoryId", marketCenterCategoryId);

                string marketCenterChildCompanyDataQuery = "EXEC usp_SelectMarketCenterChildren @categoryId";
                DataView innerMarketCenterChildCompanyDataView = manager.GetParameterizedDataView(marketCenterChildCompanyDataQuery, marketCenterChildCompanyDic);

                return innerMarketCenterChildCompanyDataView;
            });

            if(marketCenterChildCompanyDataView.Count > 0)
            {
                for (int i = 0; i < marketCenterChildCompanyDataView.Count; i++)
                {
                    MarketCenter marketCenter = new MarketCenter(Int32.Parse(marketCenterChildCompanyDataView[i]["id"].ToString()));
                    this.childCompanies.Add(marketCenter);
                }
            }

            #endregion


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
        public List<MarketCenter> childCompanies { get { return _childCompanies; } set { _childCompanies = value; } }
        public bool isTopCompany { get { return _isTopCompany; } set { _isTopCompany = value; } }
        

        public Dictionary<string, string> GetMarketCenterHtml()
        {
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            Dictionary<string, string> marketCenterTabsDict = new Dictionary<string, string>();
            List<MarketCenter> featuredMarketCenterList = new List<MarketCenter>();
            List<MarketCenter> alphaList1 = new List<MarketCenter>();
            List<MarketCenter> alphaList2 = new List<MarketCenter>();
            List<MarketCenter> alphaList3 = new List<MarketCenter>();

            //if top level and has children build popup with children links
            IList<Category> topLevelMarketCenters = categoryService.GetAllCategoriesByParentCategoryId(this.id);
            foreach (var marketcenterCategory in topLevelMarketCenters)
            {
                MarketCenter marketcenter = new MarketCenter(marketcenterCategory.Id);

                char firstLetter = marketcenter.Name.ToUpper()[0];

                //create featured company list
                if (marketcenter.isTopCompany)
                {
                    featuredMarketCenterList.Add(marketcenter);
                }

                //seperate market centers into alpha lists a-g h-p q-z
                if (firstLetter >= '0' && firstLetter <= '9')
                {
                    alphaList1.Add(marketcenter);
                }
                else if (firstLetter >= 'A' && firstLetter <= 'G')
                {
                    alphaList1.Add(marketcenter);
                }
                else if (firstLetter >= 'H' && firstLetter <= 'P')
                {
                    alphaList2.Add(marketcenter);
                }
                else if (firstLetter >= 'Q' && firstLetter <= 'Z')
                {
                    alphaList3.Add(marketcenter);
                }
                                
            }

            //call build for featured list
            //top companies only (isFeatured)
            string isfeatured = BuildFeaturedCompanyHtml(featuredMarketCenterList);

            //call build html for each alpha list      
            string alpha1 = BuildTabHtml(alphaList1);
            //string alpha2 = BuildTabHtml(alphaList2);
            //string alpha3 = BuildTabHtml(alphaList3);

            //add featured and alphas to dictionary
            marketCenterTabsDict.Add("Featured Companies", isfeatured);
            marketCenterTabsDict.Add("Companies A - G", alpha1);
            //marketCenterTabsDict.Add("Companies H - P", alpha2);
            //marketCenterTabsDict.Add("Companies Q - Z", alpha3);

            return marketCenterTabsDict;

        }
        
        private string BuildTabHtml(List<MarketCenter> marketcenterList)
        {
            string tabHtml = "";
            StringBuilder tabHtmlStringBuilder = new StringBuilder();
            //if top lvl and no children. Top level will take user to it's own page

            int numLines = 0;
            char lastChar = marketcenterList[0].Name.ToUpper()[0];

            tabHtmlStringBuilder.Append("<ul class='ul-alpha-list' >");

            if (lastChar >= '0' && lastChar <= '9')
            {
                tabHtmlStringBuilder.Append("<li class='li-alpha-header' >#</li>");
            }
            else
            {
                tabHtmlStringBuilder.Append("<li class='li-alpha-header' >" + lastChar + "</li>");
            }
            
            foreach (var marketCenter in marketcenterList)
            {

                char firstChar = marketCenter.Name.ToUpper()[0];

                if (firstChar >= 'A' && firstChar != lastChar)
                {
                    tabHtmlStringBuilder.Append("<li class='li-blank' >&nbsp;<li>");
                    tabHtmlStringBuilder.Append("<li class='li-alpha-header' >" + firstChar + "</li>");
                    numLines++;
                    lastChar = firstChar;
                }

                tabHtmlStringBuilder.Append(BuildInnerTabLink(marketCenter));
                numLines++;

                if (numLines >= 30)
                {
                    tabHtmlStringBuilder.Append("</ul>");
                    tabHtmlStringBuilder.Append("<ul class='ul-alpha-list' >");
                    numLines = 0;
                }
                
            }
            
            tabHtmlStringBuilder.Append("</ul>");

            tabHtml = tabHtmlStringBuilder.ToString();
            return tabHtml;
        }

        private string BuildInnerTabLink(MarketCenter marketcenter)
        {
            string innerLink = "";
            StringBuilder innerLinkStringBuilder = new StringBuilder();

            innerLinkStringBuilder.Append("<li>");
            innerLinkStringBuilder.Append("<a id='a-mc-link-" + marketcenter.id + "' title='" + marketcenter.Name + "' ");

            if (marketcenter.childCompanies.Count > 0)
            {
                innerLinkStringBuilder.Append("class='mc-text-link fancybox' href='#window-offices-" + marketcenter.id + "' >");
            }
            else
            {
                innerLinkStringBuilder.Append("class='mc-text-link' href='" + marketcenter.SeName + "' >");
            }

            innerLinkStringBuilder.Append(marketcenter.Name + "</a>");
            innerLinkStringBuilder.Append("/li>");

            innerLink = innerLinkStringBuilder.ToString();
            return innerLink;
        }
        
        private string BuildFeaturedCompanyHtml(List<MarketCenter> marketcenterList)
        {
            string featuredHtml = "";
            StringBuilder featuredHtmlStringBuilder = new StringBuilder();
            //if top lvl and no children. Top level will take user to it's own page
            foreach (var marketcenter in marketcenterList)
            {

                featuredHtmlStringBuilder.Append("<a id='a-mc-link-" + marketcenter.id + "' title='" + marketcenter.Name + "' ");
                if(marketcenter.childCompanies.Count > 0)
                {

                    featuredHtmlStringBuilder.Append("class='mc-img-link fancybox' href='#window-offices-" + marketcenter.id + "' ");
                    string childCompanyHtml = BuildChildCompanyHtml(marketcenter);
                    featuredHtmlStringBuilder.Append(childCompanyHtml);
                    
                }else
                {
                    featuredHtmlStringBuilder.Append("class='mc-img-link' href='" + marketcenter.SeName + "' >");
                }

                featuredHtmlStringBuilder.Append("<img src='" + marketcenter.mainPicturePath + "' alt='logo' />");
                featuredHtmlStringBuilder.Append("</a>");

            }

            featuredHtml = featuredHtmlStringBuilder.ToString();
            return featuredHtml;
        }

        private string BuildChildCompanyHtml(MarketCenter marketcenter)
        {
            string childCompanyHtml = "";
            StringBuilder childCompanyStringBuilder = new StringBuilder();

            childCompanyStringBuilder.Append("<div id='window-offices-" + marketcenter.id + "' class='dv-choose-office' >");
            childCompanyStringBuilder.Append("    <h3>Choose an Office</h3>");
            childCompanyStringBuilder.Append("    <div><label class='lbl-filter' >Search Filter:</label><input type='text' id='txt-office-filter-" + marketcenter.id + "' class='txt-office-filter' /></div>");
            childCompanyStringBuilder.Append("<div class='dv-office-list' ><ul id='ul-office-list-" + marketcenter.id + "' class='ul-office-list' >");
            childCompanyStringBuilder.Append("<li><a href='" + marketcenter.SeName + "' >" + marketcenter.Name + "</a></li>");
            foreach (var childCompany in marketcenter.childCompanies)
            {
                childCompanyStringBuilder.Append("<li><a href='" + childCompany.SeName + "' >" + childCompany.Name + "</a></li>");
            }
            childCompanyStringBuilder.Append("</div></ul>");
            childCompanyStringBuilder.Append("<div class='dv-cant-find-office' >");
            childCompanyStringBuilder.Append("<h3>Can't find your office?</h3>");
            childCompanyStringBuilder.Append("<p>Every office can use <a href='" + marketcenter.SeName + "'>these branded templates</a> or ");
            //childCompanyStringBuilder.Append("<a href='/marketcenter/market-center-request.aspx' >request templates</a> for your office.</p>"); //not sure about this now
            childCompanyStringBuilder.Append("</div></div>");

            childCompanyHtml = childCompanyStringBuilder.ToString();
            return childCompanyHtml;
        }


    }
    
}
