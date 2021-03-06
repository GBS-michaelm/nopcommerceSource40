﻿using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Nop.Web.Infrastructure.MarketCenter
{
    public class CompanyWeb : CategoryModel
    {
        DBManager manager = new DBManager();
       
        private int _id = 0;
        private int _parentCategoryId = 0;    
        private string _h1 = "";
        private string _h2 = "";
        private bool _isVisible = false;
        private bool _isDisplayLogo = false;      
        private string _logoPicturePath = "";
        private string _aboutYourMarketCenter = "";       
        private string _forgroundColor = "#000000";
        private List<int> _childCompanyIds = new List<int>();
        
        public CompanyWeb(int companyId)
        {
            //nop category data
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            this.id = companyId;
            Category category = categoryService.GetCategoryById(companyId);
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

            //datalook up via category(company) id from gbscategory table
            //Dictionary<string, Object> companyDic = new Dictionary<string, Object>();
            //companyDic.Add("@CategoryId", companyId);

            //string companyDataQuery = "EXEC usp_SelectCompanyExtendedData @CategoryId";
            //DataView companyDataView = manager.GetParameterizedDataView(companyDataQuery, companyDic);

            ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            DataView companyDataView = cacheManager.Get("company" + companyId, 60, () => {
                Dictionary<string, Object> companyDic = new Dictionary<string, Object>();
                companyDic.Add("@CategoryId", companyId);

                string companyDataQuery = "EXEC usp_SelectCompanyExtendedData @CategoryId";
                DataView innerCompanyDataView = manager.GetParameterizedDataView(companyDataQuery, companyDic);

                return innerCompanyDataView;
            });

            DataView childCompanyDataView = cacheManager.Get("childCompanies" + companyId, 60, () => {
                Dictionary<string, Object> childCategoryDic = new Dictionary<string, Object>();
                childCategoryDic.Add("@CategoryId", companyId);

                string childCategoryDataQuery = "EXEC usp_SelectGBSChildCategoryData @CategoryId";
                DataView innerChildCompanyDataView = manager.GetParameterizedDataView(childCategoryDataQuery, childCategoryDic);

                return innerChildCompanyDataView;
                 
            });

            if (companyDataView.Count > 0)
            {                
                this.parentCategoryId = category.ParentCategoryId;               
                this.h1 = !string.IsNullOrEmpty(companyDataView[0]["H1"].ToString()) ? companyDataView[0]["H1"].ToString() : this.Name;
                this.h2 = !string.IsNullOrEmpty(companyDataView[0]["H2"].ToString()) ? companyDataView[0]["H2"].ToString() : _h2;
                this.isVisible = (bool)companyDataView[0]["IsVisible"];
                this.isDisplayLogo = (bool)companyDataView[0]["IsDisplayLogo"];
                this.logoPicturePath = !string.IsNullOrEmpty(companyDataView[0]["LogoPicturePath"].ToString()) ? companyDataView[0]["LogoPicturePath"].ToString() : _logoPicturePath;
                this.aboutYourMarketCenter = !string.IsNullOrEmpty(companyDataView[0]["aboutYourMarketCenter"].ToString()) ? companyDataView[0]["aboutYourMarketCenter"].ToString() : _aboutYourMarketCenter;               
                this.foregroundColor = !string.IsNullOrEmpty(companyDataView[0]["ForegroundColor"].ToString()) ? companyDataView[0]["ForegroundColor"].ToString() : _forgroundColor;
            }

            //list of all child company ids that match the company typeid 1
            if (childCompanyDataView.Count > 0)
            {
                for (int i = 0; i < childCompanyDataView.Count; i++)
                {
                    childCompanyIds.Add(Int32.Parse(childCompanyDataView[i]["categoryId"].ToString()));
                }
            }
        }
        
        public int id { get { return _id; } set { _id = value; } }
        public int parentCategoryId { get { return _parentCategoryId; } set { _parentCategoryId = value; } }     
        public string h1 { get { return _h1; } set { _h1 = value; } }
        public string h2 { get { return _h2; } set { _h2 = value; } }   
        public bool isVisible { get { return _isVisible; } set { _isVisible = value; } }
        public bool isDisplayLogo { get { return _isDisplayLogo; } set { _isDisplayLogo = value; } }
        public string logoPicturePath { get { return _logoPicturePath; } set { _logoPicturePath = value; } }
        public string aboutYourMarketCenter { get { return _aboutYourMarketCenter; } set { _aboutYourMarketCenter = value; } }
        public string foregroundColor { get { return _forgroundColor; } set { _forgroundColor = value; } }
        public List<int> childCompanyIds { get { return _childCompanyIds; } set { _childCompanyIds = value; } }
    }
}
