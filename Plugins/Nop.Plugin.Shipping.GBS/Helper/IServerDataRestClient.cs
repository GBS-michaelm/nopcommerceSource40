using Nop.Plugin.Shipping.GBS.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.GBS.Helper
{
  public interface IServerDataRestClient
    {
        // TBL Nop Product CURD Functionality
        string AddShippingCategory(ProductGBSModel ProductModel);
       // void DeleteShippingCategory(int id);
        IEnumerable<ProductGBSModel> GetAllShippingCategories();
        ProductGBSModel GetShippingCategoryByID(int id);
        string UpdateShippingCategory(ProductGBSModel ProductModel);

        // TBL Shipping Class CURD Functionality

        //void AddShippingClass(NopProductModel nopProductModel);
        //void DeleteShippingClass(int id);
        //IEnumerable<ShippingClassModel> GetAllShippingClasses();
        //ShippingClassModel GetShippingClassByID(int id);

        //void UpdateShippingClass(NopProductModel nopProductModel);

        // TBL Zip To Ship CURD Functionality

        // void AddZipToShip(NopProductModel nopProductModel);
        //  void DeleteZipToShip(int id);

        //IEnumerable<ZipToShipModel> GetAllZipCodes();
        //ZipToShipModel GetByZipCode(string zipCode);

        // void UpdateZipToShip(NopProductModel nopProductModel);

    }
}
