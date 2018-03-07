using System;
using System.Linq;
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
using Newtonsoft.Json;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
    public class MarketCenter : CategoryModel
    {

        DBManager manager = new DBManager();
        ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
        ICacheManager cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        ICatalogModelFactory catalogModelFactory = EngineContext.Current.Resolve<ICatalogModelFactory>();
        IPictureService pictureService = EngineContext.Current.Resolve<IPictureService>();
        ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();

        
        int _id = 0;
        int _parentCategoryId = 0;
        string _h1 = "h1";
        string _h2 = "h2";
        string _topText = "top text";
        string _bottomText = "bottom text";
        string _backgroundImage = "";
        string _foregroundImage = "";
        string _backgroundColor = "";
        string _mainPicturePath;
        string _fontColor = "";
        //string _gatewayHtml = "";
        bool _isTopCompany = false;
        List<MarketCenter> _childCompanies = new List<MarketCenter>();

        //market center type link changing stuff
        IList<ProductSpecificationAttribute> specAttrList = new List<ProductSpecificationAttribute>();
        int specAttributeId;
        int specAttributeValueOption;
        int seCategoryId = 0;


        public MarketCenter(int marketCenterCategoryId, bool lightVer = true, bool getChildren = false, string customType = "")
        {
            
            if(marketCenterCategoryId != 0)
            {
                this.id = marketCenterCategoryId;
                Category category = categoryService.GetCategoryById(marketCenterCategoryId);
                CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
                catalogPagingFilteringModel.PageSize = 1;
                CategoryModel categoryModel = catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);
                this.Name = categoryModel.Name;
                this.parentCategoryId = _parentCategoryId;
                this.mainPicturePath = pictureService.GetPictureUrl(category.PictureId);


                //get spec attribute id and spec attribute option value id
                if (!string.IsNullOrEmpty(customType))
                {
                    specAttrList = specService.GetProductSpecificationAttributes(productId: 0);
                    foreach (var attr in specAttrList)
                    {
                        string typeOptionValue = "";
                        if (attr.SpecificationAttributeOption.SpecificationAttribute.Name == "Market Center Gateway Type")
                        {
                            specAttributeId = attr.SpecificationAttributeOption.SpecificationAttribute.Id;
                            typeOptionValue = attr.SpecificationAttributeOption.Name;
                            if (typeOptionValue == customType)
                            {
                                specAttributeValueOption = attr.SpecificationAttributeOption.Id;
                                break;
                            }
                        }
                    }

                    //nop check for children categories, query with children category ids to see if any are in the prod attr mapping table
                    //check if market center is at top level for office via query

                    IList<Category> categoriesList = categoryService.GetAllCategoriesByParentCategoryId(marketCenterCategoryId);

                    for (int x = 0; x <= categoriesList.Count; x++)
                    {
                        DataView marketCenterSeView = cacheManager.Get("marketCenterSeLink" + marketCenterCategoryId, 60, () => {
                            Dictionary<string, Object> marketCenterSeDic = new Dictionary<string, Object>();
                            //int catListId = categoriesList[x].Id;

                            marketCenterSeDic.Add("@CategoryId", categoriesList[x].Id);
                            marketCenterSeDic.Add("@SpecificationAttributeOptionId", specAttributeValueOption);

                            string marketCenterSeDataQuery = "EXEC usp_SelectGBSMarketCenterCustomTypeCategoryId @CategoryId, @SpecificationAttributeOptionId";
                            DataView innerMarketCenterSeDataView = manager.GetParameterizedDataView(marketCenterSeDataQuery, marketCenterSeDic);

                            return innerMarketCenterSeDataView;
                        });

                        if (marketCenterSeView.Count > 0)
                        {
                            seCategoryId = Int32.Parse(marketCenterSeView[x]["tblNopCategoryId"].ToString());
                            break;
                        }

                    }

                    if (seCategoryId == 0)
                    {

                        //check child count to see if is top office
                        int childMarketCenterCount = GetChildCategories(marketCenterCategoryId, customType, true);

                        if (childMarketCenterCount > 0)
                        {
                            this.SeName = ""; //parent market center child offices will hold links to pages
                        }
                        else
                        {
                            //generate some sort of link that knows what kind of product is missing and where to go
                            this.SeName = "/request-marketcenter-product?type=" + customType + "&company=" + marketCenterCategoryId;//page for market center products that don't exist
                        }

                    }
                    else
                    {
                        Category seCustomLinkCategory = categoryService.GetCategoryById(seCategoryId);
                        CategoryModel seCustomLinkCategoryModel = catalogModelFactory.PrepareCategoryModel(seCustomLinkCategory, catalogPagingFilteringModel);

                        this.SeName = seCustomLinkCategoryModel.SeName;

                    }

                }
                else
                {
                    this.SeName = categoryModel.SeName;
                }



                if (lightVer == false)
                {
                    this.PagingFilteringContext = catalogPagingFilteringModel;
                    this.Description = categoryModel.Description;
                    this.MetaKeywords = categoryModel.MetaKeywords;
                    this.MetaDescription = categoryModel.MetaDescription;
                    this.MetaTitle = categoryModel.MetaTitle;
                    this.PictureModel = categoryModel.PictureModel;
                    this.PagingFilteringContext = categoryModel.PagingFilteringContext;
                    this.DisplayCategoryBreadcrumb = categoryModel.DisplayCategoryBreadcrumb;
                    this.CategoryBreadcrumb = categoryModel.CategoryBreadcrumb;
                    this.SubCategories = categoryModel.SubCategories;
                    this.FeaturedProducts = categoryModel.FeaturedProducts;
                    this.Products = categoryModel.Products;
                }



                DataView marketCenterDataView = cacheManager.Get("marketCenter" + marketCenterCategoryId, 60, () => {
                    Dictionary<string, Object> marketCenterDic = new Dictionary<string, Object>();
                    marketCenterDic.Add("@CategoryId", marketCenterCategoryId);

                    string marketCenterDataQuery = "EXEC usp_SelectGBSCustomCategoryData @categoryId";
                    DataView innerMarketCenterDataView = manager.GetParameterizedDataView(marketCenterDataQuery, marketCenterDic);

                    return innerMarketCenterDataView;
                });

                if (marketCenterDataView.Count > 0)
                {
                    if (lightVer == false)
                    {
                        this.parentCategoryId = category.ParentCategoryId;
                        this.h1 = !string.IsNullOrEmpty(marketCenterDataView[0]["H1"].ToString()) ? marketCenterDataView[0]["H1"].ToString() : this.Name;
                        this.h2 = !string.IsNullOrEmpty(marketCenterDataView[0]["H2"].ToString()) ? marketCenterDataView[0]["H2"].ToString() : _h2;
                        this.topText = !string.IsNullOrEmpty(marketCenterDataView[0]["UpperText"].ToString()) ? marketCenterDataView[0]["UpperText"].ToString() : _topText;
                        this.bottomText = !string.IsNullOrEmpty(marketCenterDataView[0]["LowerText"].ToString()) ? marketCenterDataView[0]["LowerText"].ToString() : _bottomText;
                        this.backgroundImage = !string.IsNullOrEmpty(marketCenterDataView[0]["BackgroundPicturePath"].ToString()) ? marketCenterDataView[0]["BackgroundPicturePath"].ToString() : _backgroundImage;
                        this.foregroundImage = !string.IsNullOrEmpty(marketCenterDataView[0]["ForegroundPicturePath"].ToString()) ? marketCenterDataView[0]["ForegroundPicturePath"].ToString() : _foregroundImage;
                        this.backgroundColor = !string.IsNullOrEmpty(marketCenterDataView[0]["BackgroundColor"].ToString()) ? marketCenterDataView[0]["BackgroundColor"].ToString() : _backgroundColor;
                    }

                    //marketcenter custom data      

                    this.isTopCompany = !string.IsNullOrEmpty(marketCenterDataView[0]["IsFeatured"].ToString()) ? Convert.ToBoolean(marketCenterDataView[0]["IsFeatured"]) : isTopCompany = _isTopCompany;
                    if (this.isTopCompany)
                    {
                        if (!string.IsNullOrEmpty(marketCenterDataView[0]["LogoPicturePath"].ToString()))
                        {
                            this.mainPicturePath = marketCenterDataView[0]["LogoPicturePath"].ToString();
                        }
                        else
                        {
                            this.mainPicturePath = !string.IsNullOrEmpty(marketCenterDataView[0]["MainPicturePath"].ToString()) ? marketCenterDataView[0]["MainPicturePath"].ToString() : _mainPicturePath;
                        }
                    }

                }

                #region child companies
                //if (getChildren)
                //{
                //    GetChildCategories(marketCenterCategoryId, customType);
                //}            
                #endregion
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
        public string fontColor { get { return _fontColor; } set { _fontColor = value; } }
        public List<MarketCenter> childCompanies { get { return _childCompanies; } set { _childCompanies = value; } }
        public bool isTopCompany { get { return _isTopCompany; } set { _isTopCompany = value; } }
        
        private int GetChildCategories(int marketCenterCategoryId, string type, bool countOnly = false)
        {
            //child companies handling
            DataView marketCenterChildCompanyDataView = cacheManager.Get("marketCenterChildCompany" + marketCenterCategoryId, 60, () => {
                Dictionary<string, Object> marketCenterChildCompanyDic = new Dictionary<string, Object>();
                marketCenterChildCompanyDic.Add("@CategoryId", marketCenterCategoryId);

                string marketCenterChildCompanyDataQuery = "EXEC usp_SelectGBSMarketCenterChildren @categoryId";
                DataView innerMarketCenterChildCompanyDataView = manager.GetParameterizedDataView(marketCenterChildCompanyDataQuery, marketCenterChildCompanyDic);

                return innerMarketCenterChildCompanyDataView;
            });

            if (!countOnly)
            {
                if (marketCenterChildCompanyDataView.Count > 0)
                {
                    for (int i = 0; i < marketCenterChildCompanyDataView.Count; i++)
                    {
                        MarketCenter marketCenter = new MarketCenter(Int32.Parse(marketCenterChildCompanyDataView[i]["id"].ToString()), customType: type);
                        this.childCompanies.Add(marketCenter);
                    }
                }
            }
            
            return marketCenterChildCompanyDataView.Count;

        }

        public Dictionary<string, string> GetMarketCenterHtml(string type, bool hack)
        {
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            Dictionary<string, string> marketCenterTabsDict = new Dictionary<string, string>();
            List<MarketCenter> featuredMarketCenterList = new List<MarketCenter>();
            List<MarketCenter> alphaList1 = new List<MarketCenter>();
            List<MarketCenter> alphaList2 = new List<MarketCenter>();
            List<MarketCenter> alphaList3 = new List<MarketCenter>();

            //if top level and has children build popup with children links
            IList<Category> TopLevelMarketCenterIds = null;
            //check if using regular market center



            //option 2 db query then construct market centers from data, rest should be the same.

            //int x = 0;
            //int y = 0;
            //int z = 0;


            if (!hack)
            {
                for (int i = 0; i < 20; i++)
                //foreach (var marketcenterCategory in TopLevelMarketCenterIdsInOrder)
                {

                    TopLevelMarketCenterIds = categoryService.GetAllCategoriesByParentCategoryId(this.id);

                    IList<Category> TopLevelMarketCenterIdsInOrder = TopLevelMarketCenterIds.OrderBy(c => c.Name).ToList();

                    MarketCenter marketcenter = null;


                    //TESTING !!!!!!!!!--------------------------------------------------------
                    //Random r = new Random();
                    //int v = r.Next(0, 2500); // to get random pool of market centers

                    //TESTING FOR FEATURED
                    //if (i == 0)
                    //{
                    //    marketcenter = new MarketCenter(1618, getChildren: true);
                    //}
                    //else if (i == 2)
                    //{
                    //    marketcenter = new MarketCenter(1461, getChildren: true);
                    //}
                    //else
                    //{
                    marketcenter = new MarketCenter(TopLevelMarketCenterIdsInOrder[i].Id, getChildren: true);
                    //marketcenter = new MarketCenter(marketcenterCategory.Id, getChildren: true, customType: type);
                    //}                   
                    //FORCE FEATURED                     

                    char firstLetter = marketcenter.Name.ToUpper()[0];

                    //create featured company list
                    if (marketcenter.isTopCompany) // && featuredMarketCenterList.Count <= 25
                    {
                        featuredMarketCenterList.Add(marketcenter);
                    }

                    //seperate market centers into alpha lists a-g h-p q-z
                    if (firstLetter >= '0' && firstLetter <= '9') //&& alphaList1.Count <= 100
                    {
                        alphaList1.Add(marketcenter); //x++;
                    }
                    else if (firstLetter >= 'A' && firstLetter <= 'G') //&& alphaList1.Count <= 100
                    {
                        alphaList1.Add(marketcenter);// x++;
                    }
                    else if (firstLetter >= 'H' && firstLetter <= 'P') // && alphaList2.Count <= 25
                    {
                        alphaList2.Add(marketcenter); //y++;
                    }
                    else if (firstLetter >= 'Q' && firstLetter <= 'Z') // && alphaList3.Count <= 25
                    {
                        alphaList3.Add(marketcenter); //z++;
                    }

                    //TESTING SECTION --------------------limit number of market centers to load page faster
                    //if (featuredMarketCenterList.Count > 25 && alphaList1.Count > 25 && alphaList2.Count > 25 && alphaList3.Count > 25)
                    //{
                    //    break;
                    //}
                    //TESTING --------------------------------------------------------------------------



                }
            }          
            else
            {
                //hacking

                featuredMarketCenterList = GetTabData(true);
                alphaList1 = GetTabData(null, '!', 'G');
                alphaList2 = GetTabData(null, 'H', 'P');
                alphaList3 = GetTabData(null, 'Q', 'Z');

            }

            //call build for featured list
            //top companies only (isFeatured)
            string isfeatured = featuredMarketCenterList.Count > 0 ? BuildFeaturedCompanyHtml(featuredMarketCenterList) : "";
            string childLinksHtml = featuredMarketCenterList.Count > 0 ? BuildChildCompanyHtml(featuredMarketCenterList) : "";

            //call build html for each alpha list      
            string alpha1 = alphaList1.Count > 0 ? BuildTabHtml(alphaList1) : "";
            string alpha2 = alphaList2.Count > 0 ? BuildTabHtml(alphaList2) : "";
            string alpha3 = alphaList3.Count > 0 ? BuildTabHtml(alphaList3) : "";
            string cantFindHtml = BuildCantFindHtml();

            //add featured and alphas to dictionary
            marketCenterTabsDict.Add("Featured Companies", isfeatured);
            marketCenterTabsDict.Add("Companies A - G", alpha1);
            marketCenterTabsDict.Add("Companies H - P", alpha2);
            marketCenterTabsDict.Add("Companies Q - Z", alpha3);
            marketCenterTabsDict.Add("Can't Find Your Company?", cantFindHtml);
            marketCenterTabsDict.Add("HiddenHtml", childLinksHtml);

            return marketCenterTabsDict;

        }
        
        private List<MarketCenter> GetTabData(bool? isFeatured = null, char start = '!', char end = 'z')
        {

            List<MarketCenter> marketCenters = null;
            Dictionary<string, Object> marketCenterTabDic = new Dictionary<string, Object>();            
            marketCenterTabDic.Add("@start", start);
            marketCenterTabDic.Add("@end", end);

            string select = "EXEC usp_SELECTGBSGetMarketCenters @start, @end";
            if (isFeatured != null)
            {
                select = "EXEC usp_SELECTGBSGetMarketCenters  @start, @end, @isFeatured";
                marketCenterTabDic.Add("@isFeatured", isFeatured);
            }
                       
            string jsonResult = manager.GetParameterizedJsonString(select, marketCenterTabDic);
            
            marketCenters = JsonConvert.DeserializeObject<List<MarketCenter>>(jsonResult);

            return marketCenters;

        }


        private string BuildTabHtml(List<MarketCenter> marketcenterList)
        {
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

                if (numLines == ((marketcenterList.Count / 2) + 1))
                {
                    tabHtmlStringBuilder.Append("</ul>");
                    tabHtmlStringBuilder.Append("<ul class='ul-alpha-list' >");
                    //numLines = 0;
                }

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

            }
            
            tabHtmlStringBuilder.Append("</ul>");
                        
            return tabHtmlStringBuilder.ToString(); ;
        }

        private string BuildInnerTabLink(MarketCenter marketcenter)
        {
            StringBuilder innerLinkStringBuilder = new StringBuilder();

            innerLinkStringBuilder.Append("<li>");
            innerLinkStringBuilder.Append("<a id='a-mc-link-" + marketcenter.id + "' title='" + marketcenter.Name + "' ");

            if (marketcenter.childCompanies.Count > 0)
            {
                innerLinkStringBuilder.Append("class='mc-text-link box-handle' href='#window-offices-" + marketcenter.id + "' >");
            }
            else
            {
                innerLinkStringBuilder.Append("class='mc-text-link' href='" + marketcenter.SeName + "' >");
            }

            innerLinkStringBuilder.Append(marketcenter.Name + "</a>");
            innerLinkStringBuilder.Append("</li>");

            return innerLinkStringBuilder.ToString(); ;
        }
        
        private string BuildFeaturedCompanyHtml(List<MarketCenter> marketcenterList)
        {
            StringBuilder featuredHtmlStringBuilder = new StringBuilder();
            //if top lvl and no children. Top level will take user to it's own page
            foreach (var marketcenter in marketcenterList)
            {

                featuredHtmlStringBuilder.Append("<a id='a-mc-link-" + marketcenter.id + "' title='" + marketcenter.Name + "' ");
                if(marketcenter.childCompanies.Count > 0)
                {
                    featuredHtmlStringBuilder.Append("class='mc-img-link box-handle' href='#window-offices-" + marketcenter.id + "' >");
                    if (string.IsNullOrEmpty(marketcenter.mainPicturePath))
                    {
                        featuredHtmlStringBuilder.Append("<p style='color: " + marketcenter.fontColor + "'>" + marketcenter.Name + "' </p>");
                    }
                    else
                    {
                        featuredHtmlStringBuilder.Append("<img src='" + marketcenter.mainPicturePath + "' data-toggle='modal' alt='logo' />");
                    }
                                     
                    featuredHtmlStringBuilder.Append("</a>");                 
                }
                else
                {
                    featuredHtmlStringBuilder.Append("class='mc-img-link' href='" + marketcenter.SeName + "' >");
                    if (string.IsNullOrEmpty(marketcenter.mainPicturePath))
                    {
                        featuredHtmlStringBuilder.Append("<p style='color: " + marketcenter.fontColor + "'>" + marketcenter.Name + "' </p>");
                    }
                    else
                    {
                        featuredHtmlStringBuilder.Append("<img src='" + marketcenter.mainPicturePath + "' alt='logo' />");
                    }
                    
                    featuredHtmlStringBuilder.Append("</a>");
                }           

            }

            return featuredHtmlStringBuilder.ToString(); ;
        }

        private string BuildChildCompanyHtml(List<MarketCenter> marketcenterList)
        {
            StringBuilder childCompanyStringBuilder = new StringBuilder();

            foreach (var marketcenter in marketcenterList)
            {
                if(marketcenter.childCompanies.Count > 0)
                {
                    //hide all children html for fancy box to use

                    childCompanyStringBuilder.Append("    <div id='window-offices-" + marketcenter.id + "' class='dv-choose-office' >");                    
                    childCompanyStringBuilder.Append("        <h3>Choose an Office</h3>");
                    childCompanyStringBuilder.Append("        <div class='search-filter-wrap'><label class='lbl-filter' >Search Filter:</label><input type='text' id='txt-office-filter-" + marketcenter.id + "' class='txt-office-filter' onkeyup='SearchCall(this)' /></div>");
                    childCompanyStringBuilder.Append("        <div class='dv-office-list' ><ul id='ul-office-list-" + marketcenter.id + "' class='ul-office-list' >");
                    childCompanyStringBuilder.Append("            <li><a href='" + marketcenter.SeName + "' >" + marketcenter.Name + "</a></li>");
                    foreach (var childCompany in marketcenter.childCompanies)
                    {
                        childCompanyStringBuilder.Append("        <li><a href='" + childCompany.SeName + "' >" + childCompany.Name + "</a></li>");
                    }
                    childCompanyStringBuilder.Append("        </div></ul>");
                    childCompanyStringBuilder.Append("        <div class='dv-cant-find-office' >");
                    childCompanyStringBuilder.Append("            <h3>Can't find your office?</h3>");
                    childCompanyStringBuilder.Append("            <p>Every office can use <a href='' class='branded-link'>these branded templates</a> or ");
                    childCompanyStringBuilder.Append("            <a href='/marketcenter/market-center-request.aspx' class='request-link' >request templates</a> for your office.</p>"); //not sure about this now
                    childCompanyStringBuilder.Append("        </div>");
                    childCompanyStringBuilder.Append("    </div>");

                }
            }          
            
            return childCompanyStringBuilder.ToString(); ;
        }


        private string BuildCantFindHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id='cantFindTab'>");
            sb.Append("    <p>Ready to make the personalizing, proofing, and purchasing of essential marketing tools for your office a whole lot easier?</p>");
            sb.Append("    <p>If your office has more than 50 agents / employees and a commitment from leadership to co-market the solution, you qualify for your own custom Market Center.</p>");
            sb.Append("    <a id='btn-request-mc' href='' >Request a Market Center</a>");
            sb.Append("</div>");

            return sb.ToString();
        }


    }
    
}





//public MarketCenter(int marketCenterCategoryId, bool lightVer = true, bool getChildren = false, string customType = "")
//{

//    this.id = marketCenterCategoryId;
//    Category category = categoryService.GetCategoryById(marketCenterCategoryId);
//    CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
//    catalogPagingFilteringModel.PageSize = 1;
//    CategoryModel categoryModel = catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);
//    this.Name = categoryModel.Name;
//    this.parentCategoryId = _parentCategoryId;
//    this.mainPicturePath = pictureService.GetPictureUrl(category.PictureId);


//    //get spec attribute id and spec attribute option value id
//    if (!string.IsNullOrEmpty(customType))
//    {
//        specAttrList = specService.GetProductSpecificationAttributes(productId: 0);
//        foreach (var attr in specAttrList)
//        {
//            string typeOptionValue = "";
//            if (attr.SpecificationAttributeOption.SpecificationAttribute.Name == "Market Center Gateway Type")
//            {
//                specAttributeId = attr.SpecificationAttributeOption.SpecificationAttribute.Id;
//                typeOptionValue = attr.SpecificationAttributeOption.Name;
//                if (typeOptionValue == customType)
//                {
//                    specAttributeValueOption = attr.SpecificationAttributeOption.Id;
//                    break;
//                }
//            }
//        }

//        //nop check for children categories, query with children category ids to see if any are in the prod attr mapping table
//        //check if market center is at top level for office via query

//        IList<Category> categoriesList = categoryService.GetAllCategoriesByParentCategoryId(marketCenterCategoryId);

//        for (int x = 0; x <= categoriesList.Count; x++)
//        {
//            DataView marketCenterSeView = cacheManager.Get("marketCenterSeLink" + marketCenterCategoryId, 60, () => {
//                Dictionary<string, Object> marketCenterSeDic = new Dictionary<string, Object>();
//                //int catListId = categoriesList[x].Id;

//                marketCenterSeDic.Add("@CategoryId", categoriesList[x].Id);
//                marketCenterSeDic.Add("@SpecificationAttributeOptionId", specAttributeValueOption);

//                string marketCenterSeDataQuery = "EXEC usp_SelectGBSMarketCenterCustomTypeCategoryId @CategoryId, @SpecificationAttributeOptionId";
//                DataView innerMarketCenterSeDataView = manager.GetParameterizedDataView(marketCenterSeDataQuery, marketCenterSeDic);

//                return innerMarketCenterSeDataView;
//            });

//            if (marketCenterSeView.Count > 0)
//            {
//                seCategoryId = Int32.Parse(marketCenterSeView[x]["tblNopCategoryId"].ToString());
//                break;
//            }

//        }

//        if (seCategoryId == 0)
//        {

//            //check child count to see if is top office
//            int childMarketCenterCount = GetChildCategories(marketCenterCategoryId, customType, true);

//            if (childMarketCenterCount > 0)
//            {
//                this.SeName = ""; //parent market center child offices will hold links to pages
//            }
//            else
//            {
//                //generate some sort of link that knows what kind of product is missing and where to go
//                this.SeName = "/request-marketcenter-product?type=" + customType + "&company=" + marketCenterCategoryId;//page for market center products that don't exist
//            }

//        }
//        else
//        {
//            Category seCustomLinkCategory = categoryService.GetCategoryById(seCategoryId);
//            CategoryModel seCustomLinkCategoryModel = catalogModelFactory.PrepareCategoryModel(seCustomLinkCategory, catalogPagingFilteringModel);

//            this.SeName = seCustomLinkCategoryModel.SeName;

//        }

//    }
//    else
//    {
//        this.SeName = categoryModel.SeName;
//    }



//    if (lightVer == false)
//    {
//        this.PagingFilteringContext = catalogPagingFilteringModel;
//        this.Description = categoryModel.Description;
//        this.MetaKeywords = categoryModel.MetaKeywords;
//        this.MetaDescription = categoryModel.MetaDescription;
//        this.MetaTitle = categoryModel.MetaTitle;
//        this.PictureModel = categoryModel.PictureModel;
//        this.PagingFilteringContext = categoryModel.PagingFilteringContext;
//        this.DisplayCategoryBreadcrumb = categoryModel.DisplayCategoryBreadcrumb;
//        this.CategoryBreadcrumb = categoryModel.CategoryBreadcrumb;
//        this.SubCategories = categoryModel.SubCategories;
//        this.FeaturedProducts = categoryModel.FeaturedProducts;
//        this.Products = categoryModel.Products;
//    }



//    DataView marketCenterDataView = cacheManager.Get("marketCenter" + marketCenterCategoryId, 60, () => {
//        Dictionary<string, Object> marketCenterDic = new Dictionary<string, Object>();
//        marketCenterDic.Add("@CategoryId", marketCenterCategoryId);

//        string marketCenterDataQuery = "EXEC usp_SelectGBSCustomCategoryData @categoryId";
//        DataView innerMarketCenterDataView = manager.GetParameterizedDataView(marketCenterDataQuery, marketCenterDic);

//        return innerMarketCenterDataView;
//    });

//    if (marketCenterDataView.Count > 0)
//    {
//        if (lightVer == false)
//        {
//            this.parentCategoryId = category.ParentCategoryId;
//            this.h1 = !string.IsNullOrEmpty(marketCenterDataView[0]["H1"].ToString()) ? marketCenterDataView[0]["H1"].ToString() : this.Name;
//            this.h2 = !string.IsNullOrEmpty(marketCenterDataView[0]["H2"].ToString()) ? marketCenterDataView[0]["H2"].ToString() : _h2;
//            this.topText = !string.IsNullOrEmpty(marketCenterDataView[0]["UpperText"].ToString()) ? marketCenterDataView[0]["UpperText"].ToString() : _topText;
//            this.bottomText = !string.IsNullOrEmpty(marketCenterDataView[0]["LowerText"].ToString()) ? marketCenterDataView[0]["LowerText"].ToString() : _bottomText;
//            this.backgroundImage = !string.IsNullOrEmpty(marketCenterDataView[0]["BackgroundPicturePath"].ToString()) ? marketCenterDataView[0]["BackgroundPicturePath"].ToString() : _backgroundImage;
//            this.foregroundImage = !string.IsNullOrEmpty(marketCenterDataView[0]["ForegroundPicturePath"].ToString()) ? marketCenterDataView[0]["ForegroundPicturePath"].ToString() : _foregroundImage;
//            this.backgroundColor = !string.IsNullOrEmpty(marketCenterDataView[0]["BackgroundColor"].ToString()) ? marketCenterDataView[0]["BackgroundColor"].ToString() : _backgroundColor;
//        }

//        //marketcenter custom data      

//        this.isTopCompany = !string.IsNullOrEmpty(marketCenterDataView[0]["IsFeatured"].ToString()) ? Convert.ToBoolean(marketCenterDataView[0]["IsFeatured"]) : isTopCompany = _isTopCompany;
//        if (this.isTopCompany)
//        {
//            if (!string.IsNullOrEmpty(marketCenterDataView[0]["LogoPicturePath"].ToString()))
//            {
//                this.mainPicturePath = marketCenterDataView[0]["LogoPicturePath"].ToString();
//            }
//            else
//            {
//                this.mainPicturePath = !string.IsNullOrEmpty(marketCenterDataView[0]["MainPicturePath"].ToString()) ? marketCenterDataView[0]["MainPicturePath"].ToString() : _mainPicturePath;
//            }
//        }

//    }

//    #region child companies
//    //if (getChildren)
//    //{
//    //    GetChildCategories(marketCenterCategoryId, customType);
//    //}            
//    #endregion

//}
