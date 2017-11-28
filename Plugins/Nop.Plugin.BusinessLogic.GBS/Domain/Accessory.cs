using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.BusinessDataAccess.GBS;
using System.Data;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Vendors;
using Nop.Core;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Services.Events;
using Nop.Services.Common;
using Nop.Core.Caching;
using System.Web;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
    class Accessory : CategoryModel
    {
        DBManager manager = new DBManager();
        public static DBManager staticManager = new DBManager();

        int _id = 0;
        int _parentCategoryId = 0;
        string _h1 = "";
        string _mainPicturePath = "";
        int _featuredProductId = 2222;
        bool _isFeatured = false;
        //int _orderBy = 0;
        
        public Accessory(int accessoryId)
        {
            ICategoryService categoryService = EngineContext.Current.Resolve<ICategoryService>();
            this.id = accessoryId;
            Category category = categoryService.GetCategoryById(accessoryId);
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

            ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            DataView accessoryDataView = cacheManager.Get("categoryAccessory" + accessoryId, 60, () => {
                Dictionary<string, Object> accessoryDic = new Dictionary<string, Object>();
                accessoryDic.Add("@CategoryId", accessoryId);

                string accessoryDataQuery = "EXEC usp_SelectGBSCrossSellAccessory @categoryId";
                DataView innerAccessoryDataView = manager.GetParameterizedDataView(accessoryDataQuery, accessoryDic);

                return innerAccessoryDataView;
            });
            
            if(accessoryDataView.Count > 0)
            {

                this.parentCategoryId = category.ParentCategoryId;
                this.h1 = !string.IsNullOrEmpty(accessoryDataView[0]["H1"].ToString()) ? accessoryDataView[0]["H1"].ToString() : this.Name;
                this.mainPicturePath = !string.IsNullOrEmpty(accessoryDataView[0]["MainPicturePath"].ToString()) ? accessoryDataView[0]["MainPicturePath"].ToString() : _mainPicturePath;
                int featuredId;
                this.featuredProductId = Int32.TryParse(accessoryDataView[0]["FeaturedProductId"].ToString(), out featuredId) ? featuredId : _featuredProductId;
                this.isFeatured = accessoryDataView[0]["IsFeatured"] != null && accessoryDataView[0]["IsFeatured"] != DBNull.Value ? (Convert.ToBoolean(accessoryDataView[0]["IsFeatured"]) == true ? true : false) : _isFeatured;
                int order;
                //this.orderBy = Int32.TryParse(accessoryDataView[0]["OrderBy"].ToString(), out order) ? order : _orderBy;

            }
            
        }

        public int id { get { return _id; } set { _id = value; } }
        public int parentCategoryId { get { return _parentCategoryId; } set { _parentCategoryId = value; } }
        public string h1 { get { return _h1; } set { _h1 = value; } }
        public string mainPicturePath { get { return _mainPicturePath; } set { _mainPicturePath = value; } }
        public int featuredProductId { get { return _featuredProductId; } set { _featuredProductId = value; } }
        public bool isFeatured { get { return _isFeatured; } set { _isFeatured = value; } }
        //public int orderBy { get { return _orderBy; } set { _orderBy = value; } }
        
        public static List<Accessory> GetAllCrossSellAccessories(int accessoryGroupId)
        {

            List<Accessory> accessoriesList = new List<Accessory>();

            ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            DataView accessoryDataView = cacheManager.Get("categoryAccessory" + accessoryGroupId, 60, () => {
                Dictionary<string, Object> accessoryDic = new Dictionary<string, Object>();
                accessoryDic.Add("@accessoryGroupId", accessoryGroupId);

                string accessoryDataQuery = "EXEC usp_SelectGBSRelatedCrossSellCategories @accessoryGroupId"; //check type as well
                DataView innerAccessoryDataView = staticManager.GetParameterizedDataView(accessoryDataQuery, accessoryDic);

                return innerAccessoryDataView;
            });

            if(accessoryDataView.Count > 0)
            {

                for (int i = 0; i < accessoryDataView.Count; i++)
                {

                    Accessory accessory = new Accessory(Int32.Parse(accessoryDataView[i]["CategoryId"].ToString()));
                    accessoriesList.Add(accessory);

                }

            }
            
            return accessoriesList;

        }
        

    }
}
