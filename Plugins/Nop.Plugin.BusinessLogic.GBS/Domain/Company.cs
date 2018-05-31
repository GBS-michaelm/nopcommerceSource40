using System;
using System.Collections.Generic;
using Nop.Plugin.BusinessDataAccess.GBS;
using System.Data;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Core.Caching;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Core.Domain.Media;
using Newtonsoft.Json;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
        
    public class Company //: CategoryModel
    {

        DBManager manager = new DBManager();
        static DBManager MANAGER = new DBManager();
        ICacheManager cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
        static ICategoryService CATEGORYSERVICE = EngineContext.Current.Resolve<ICategoryService>();
        IPictureService pictureService = EngineContext.Current.Resolve<IPictureService>();
        static IPictureService PICTURESERVICE = EngineContext.Current.Resolve<IPictureService>();
        ILogger logger = EngineContext.Current.Resolve<ILogger>();

        private int _id = 0;
        private int _parentCategoryId = 0;    
        private string _h1 = "";
        private string _h2 = "";
        private bool _isVisible = false;
        private bool _isDisplayLogo = false;      
        private string _logoPicturePath = "";
        private string _aboutYourMarketCenter = "";
        private string _agentPacks = "";
        private string _forgroundColor = "#000000";
        private List<int> _childCompanyIds = new List<int>();
        string _name = "";
        string _description = "";
        string _seName = "";


        public static Company GetCompany(int companyId, bool lightVer = true)
        {

            Dictionary<string, Object> companyDic = new Dictionary<string, Object>();
            string select = "EXEC usp_SELECTGBSGetSingleCompany_Lean @categoryId";
            companyDic.Add("@categoryId", companyId);

            string jsonResult = MANAGER.GetParameterizedJsonString(select, companyDic);

            List<Company> marketcentersList = JsonConvert.DeserializeObject<List<Company>>(jsonResult);
            Category category = CATEGORYSERVICE.GetCategoryById(marketcentersList[0].id);
            marketcentersList[0].logoPicturePath = PICTURESERVICE.GetPictureUrl(category.PictureId);
            
            return marketcentersList[0];

        }
        
        //public Company(int companyId, bool lightVer = true)
        //{
        //    //nop category data

            //    this.id = companyId;
            //    Category category = categoryService.GetCategoryById(companyId);
            //    ICatalogModelFactory catalogModelFactory = EngineContext.Current.Resolve<ICatalogModelFactory>();           
            //    CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();            
            //    catalogPagingFilteringModel.PageSize = 1;
            //    this.PagingFilteringContext = catalogPagingFilteringModel;
            //    CategoryModel categoryModel = catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);          
            //    this.Name = categoryModel.Name;
            //    this.Description = categoryModel.Description;

            //    if (!lightVer)
            //    {
            //        this.MetaKeywords = categoryModel.MetaKeywords;
            //        this.MetaDescription = categoryModel.MetaDescription;
            //        this.MetaTitle = categoryModel.MetaTitle;
            //        this.PagingFilteringContext = categoryModel.PagingFilteringContext;
            //        this.PictureModel = categoryModel.PictureModel;
            //        this.DisplayCategoryBreadcrumb = categoryModel.DisplayCategoryBreadcrumb;
            //        this.CategoryBreadcrumb = categoryModel.CategoryBreadcrumb;
            //        this.SubCategories = categoryModel.SubCategories;
            //        this.FeaturedProducts = categoryModel.FeaturedProducts;
            //        this.Products = categoryModel.Products;
            //    }

            //    this.SeName = categoryModel.SeName;

            //    IPictureService pictureService = EngineContext.Current.Resolve<IPictureService>();
            //    string picturePath = pictureService.GetPictureUrl(category.PictureId);


            //    DataView companyDataView = cacheManager.Get("company" + companyId, 60, () => {
            //        Dictionary<string, Object> companyDic = new Dictionary<string, Object>();
            //        companyDic.Add("@CategoryId", companyId);

            //        string companyDataQuery = "EXEC usp_SelectCompanyExtendedData @CategoryId";
            //        DataView innerCompanyDataView = manager.GetParameterizedDataView(companyDataQuery, companyDic);

            //        return innerCompanyDataView;
            //    });

            //    DataView childCompanyDataView = cacheManager.Get("childCompanies" + companyId, 60, () => {
            //        Dictionary<string, Object> childCategoryDic = new Dictionary<string, Object>();
            //        childCategoryDic.Add("@CategoryId", companyId);

            //        string childCategoryDataQuery = "EXEC usp_SelectGBSChildCategoryData @CategoryId";
            //        DataView innerChildCompanyDataView = manager.GetParameterizedDataView(childCategoryDataQuery, childCategoryDic);

            //        return innerChildCompanyDataView;

            //    });

            //    if (companyDataView.Count > 0)
            //    {

            //        this.parentCategoryId = category.ParentCategoryId;               
            //        this.h1 = !string.IsNullOrEmpty(companyDataView[0]["H1"].ToString()) ? companyDataView[0]["H1"].ToString() : this.Name;
            //        this.h2 = !string.IsNullOrEmpty(companyDataView[0]["H2"].ToString()) ? companyDataView[0]["H2"].ToString() : _h2;
            //        this.isVisible = (bool)companyDataView[0]["IsVisible"];
            //        this.isDisplayLogo = (bool)companyDataView[0]["IsDisplayLogo"];
            //        //this.logoPicturePath = !string.IsNullOrEmpty(companyDataView[0]["LogoPicturePath"].ToString()) ? companyDataView[0]["LogoPicturePath"].ToString() : _logoPicturePath;
            //        this.logoPicturePath = !string.IsNullOrEmpty(picturePath) ? picturePath : "";
            //        this.aboutYourMarketCenter = !string.IsNullOrEmpty(companyDataView[0]["aboutYourMarketCenter"].ToString()) ? companyDataView[0]["aboutYourMarketCenter"].ToString() : _aboutYourMarketCenter;
            //        this.agentPacks = !string.IsNullOrEmpty(companyDataView[0]["AgentPacks"].ToString()) ? companyDataView[0]["AgentPacks"].ToString() : _agentPacks;
            //        this.foregroundColor = !string.IsNullOrEmpty(companyDataView[0]["ForegroundColor"].ToString()) ? companyDataView[0]["ForegroundColor"].ToString() : _forgroundColor;

            //    }
            //    //list of all child company ids that match the company typeid 1
            //    if (childCompanyDataView.Count > 0)
            //    {
            //        for (int i = 0; i < childCompanyDataView.Count; i++)
            //        {
            //            childCompanyIds.Add(Int32.Parse(childCompanyDataView[i]["categoryId"].ToString()));
            //        }
            //    }

            //}

        public int id { get { return _id; } set { _id = value; } }
        public int parentCategoryId { get { return _parentCategoryId; } set { _parentCategoryId = value; } }     
        public string h1 { get { return _h1; } set { _h1 = value; } }
        public string h2 { get { return _h2; } set { _h2 = value; } }   
        public bool isVisible { get { return _isVisible; } set { _isVisible = value; } }
        public bool isDisplayLogo { get { return _isDisplayLogo; } set { _isDisplayLogo = value; } }
        public string logoPicturePath { get { return _logoPicturePath; } set { _logoPicturePath = value; } }
        public string aboutYourMarketCenter { get { return _aboutYourMarketCenter; } set { _aboutYourMarketCenter = value; } }
        public string agentPacks { get { return _agentPacks; } set { _agentPacks = value; } }
        public string foregroundColor { get { return _forgroundColor; } set { _forgroundColor = value; } }
        public List<int> childCompanyIds { get { return _childCompanyIds; } set { _childCompanyIds = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string SeName { get { return _seName; } set { _seName = value; } }

        public List<Company> GetNonMarketCenterCompanyCategories(int parentCompanyId)
        {

            MediaSettings mediaSettings = EngineContext.Current.Resolve<MediaSettings>();

            List<Company> NMCcategories = new List<Company>();
            NMCcategories = cacheManager.Get("GetNonMarketCenterCompanyCategories" + parentCompanyId, 60, () =>
            {
                List<Company> categories = new List<Company>();

                try
                {
                            DataView nonMarketCenterCompanyDataView = cacheManager.Get("nonmarketcentercategory" + parentCompanyId, 60, () => {
                                Dictionary<string, Object> nonMarketCenterCompanyDic = new Dictionary<string, Object>();
                                nonMarketCenterCompanyDic.Add("@parentCategoryId", parentCompanyId);

                                string nonMarketCenterCompanyDataQuery = "EXEC usp_SelectNonMarketCenterCompanyCategories @parentCategoryId";
                                DataView innerCompanyDataView = manager.GetParameterizedDataView(nonMarketCenterCompanyDataQuery, nonMarketCenterCompanyDic);

                                return innerCompanyDataView;
                            });

                            if (nonMarketCenterCompanyDataView.Count > 0)
                            {

                                for (int i = 0; i < nonMarketCenterCompanyDataView.Count; i++)
                                {

                                    //Category category = categoryService.GetCategoryById(Int32.Parse(nonMarketCenterCompanyDataView[i]["categoryId"].ToString()));
                                    //ICatalogModelFactory catalogModelFactory = EngineContext.Current.Resolve<ICatalogModelFactory>();
                                    //CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
                                    //catalogPagingFilteringModel.PageSize = 1;
                                    //this.PagingFilteringContext = catalogPagingFilteringModel;
                                    //CategoryModel categoryModel = catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);
                                    //this.Name = categoryModel.Name;
                                    //this.Description = categoryModel.Description;
                                    //this.SeName = categoryModel.SeName;


                                    Company company = Company.GetCompany(Int32.Parse(nonMarketCenterCompanyDataView[i]["categoryId"].ToString()));

                                    
                        
                                    //IPictureService pictureService = EngineContext.Current.Resolve<IPictureService>();
                                    //string picturePath = pictureService.GetPictureUrl(category.PictureId, mediaSettings.CategoryThumbPictureSize);

                                    //this.PictureModel = categoryModel.PictureModel;
                        
                                    //DataView companyDataView = cacheManager.Get("company" + company.id, 60, () => {
                                    //    Dictionary<string, Object> companyDic = new Dictionary<string, Object>();
                                    //    companyDic.Add("@CategoryId", company.id);

                                    //    string companyDataQuery = "EXEC usp_SelectCompanyExtendedData @CategoryId";
                                    //    DataView innerCompanyDataView = manager.GetParameterizedDataView(companyDataQuery, companyDic);

                                    //    return innerCompanyDataView;
                                    //});

                                    //if (companyDataView.Count > 0)
                                    //{
                                    //    categoryModel.CustomProperties.Add("LogoPicturePath", picturePath);
                                    //    //description text stuff like pricing and stuff 
                                    //}

                                    categories.Add(company);

                                }

                            }
                        }catch(Exception ex)
                        {
                            logger.Error("Company.cs GetNonMarketCenterCompanyCategories : ", ex);
                
                            throw ex;

                        }
                return categories;
            });
                      
            return NMCcategories;
            
        }
        
    }
}
