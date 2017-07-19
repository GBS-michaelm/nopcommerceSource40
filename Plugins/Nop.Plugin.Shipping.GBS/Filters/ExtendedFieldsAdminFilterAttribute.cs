using System.Web.Mvc;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Plugin.Shipping.GBS.Models.Product;
using Nop.Plugin.DataAccess.GBS;

namespace Nop.Plugin.Shipping.GBS.Filters
{
    
    public class ExtendedFieldsAdminFilterAttribute : ActionFilterAttribute 
    {
        public string _TableName = "";

       
        public override void  OnActionExecuted(ActionExecutedContext filterContext)
        {
        
            //_TableName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Table.Name").ResourceValue.ToString();

            ProductGBSModel _nopProductModel = new ProductGBSModel();

            _nopProductModel.ProductID = int.Parse(filterContext.HttpContext.Request.Form["Id"]);

            // Get value from nop form 
            _nopProductModel.ShippingCategoryA = filterContext.HttpContext.Request.Form["ShippingCategoryA"];
            _nopProductModel.ShippingCategoryB = filterContext.HttpContext.Request.Form["ShippingCategoryB"];
      
            var product = EngineContext.Current.Resolve<IProductService>().GetProductById(_nopProductModel.ProductID);
            var Tproduct = EngineContext.Current.Resolve<IProductService>().GetProductById(1);
            //Save value for the shipping class
            if (product != null)
            {
                var _flag = product.GetAttribute<string>("NewItemFlag");
                _TableName = Tproduct.GetAttribute<string>("TableName");
                var res =  _flag == "True" ? Add(_nopProductModel) : Update(_nopProductModel);  
             }
        }

        #region Helper
        public string Add(ProductGBSModel ProductgbsModel)
        {
            DataManager.AddGBSShippingCategory(ProductgbsModel, _TableName);
            return "";  
        }
        public string Update(ProductGBSModel ProductgbsModel)
        {
            DataManager.UpdateGBSShippingCategory(ProductgbsModel, _TableName);
            return "";
        }
        #endregion
    }

}