using Nop.Web.Framework.Controllers;
using System.Web.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.BusinessLogic.GBS.Domain;
using Nop.Web.Controllers;
using Nop.Plugin.BusinessLogic.GBS.Models;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Web.Factories;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;

namespace Nop.Plugin.BusinessLogic.GBS.Controllers
{
    
    public class AccessoryPageController : BaseController
    {
                
        public ActionResult AccessoryPage(int groupId, int productId)
        {
            AccessoryPageModel model = new AccessoryPageModel();
            model.groupId = groupId;
            model.productId = productId;

            return View("AccessoryPage", model);
        }

        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult AccessoryCategories(int groupId)
        {
            var iCategoryService = EngineContext.Current.Resolve<ICategoryService>();

            AccessoryPageCategoryAccessoriesModel accessoryCategoryBlocks = new AccessoryPageCategoryAccessoriesModel();
            List<Accessory> accessoryList = new List<Accessory>();
            accessoryList = Accessory.GetAllCrossSellAccessories(groupId); //accessory categories that relate to selected product

            var accessoriesByDisplayOrderList = accessoryList.OrderBy(x => x.displayOrder).ToList();

            if (accessoryList.Count > 0)
            {
                foreach (var accessory in accessoriesByDisplayOrderList)
                {
                    
                    decimal price = 0;
                    IPagedList<ProductCategory> productCategoryList = iCategoryService.GetProductCategoriesByCategoryId(accessory.id);
                    var productsInOrder = productCategoryList.Where(y => y.Product.Price > 0).OrderBy(x => x.Product.Price).ToList();
                    for (int p = 0; p < productsInOrder.Count; p++)
                    {
                        if (productsInOrder[p].Product.Price > 0)
                        {
                            price = productsInOrder[p].Product.Price;
                            break;
                        }
                    }

                    //price = 0; // for testing log
                    if(price != 0)
                    {
                        AccessoryPageBoxModel accessoryBox = new AccessoryPageBoxModel();
                        accessoryBox.name = accessory.Name;
                        accessoryBox.mainPicturePath = accessory.mainPicturePath;
                        accessoryBox.price = price;
                        accessoryBox.isFeatured = accessory.isFeatured;
                        accessoryBox.description = accessory.Description;
                        accessoryBox.featuredProductId = accessory.featuredProductId;
                        accessoryBox.categoryPageLink = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/" + accessory.SeName;
                        accessoryBox.displayOrder = accessory.displayOrder;

                        accessoryCategoryBlocks.accessoryBoxes.Add(accessoryBox);
                    }else
                    {
                        //log error if no pricing is found
                        Exception ex = new Exception("Accessory Failed To Load. Category Id: " + accessory.id + ". All Products in category are missing pricing");
                        base.LogException(ex);                       
                    }                 

                }
            }
            
            




            return View("AccessoryCategoryAccessories", accessoryCategoryBlocks);
        }


        public ActionResult AccessoryCrossSells(int productId, int? productThumbPictureSize)
        {
            IProductService productService = EngineContext.Current.Resolve<IProductService>();
            IProductModelFactory productModelFactory = EngineContext.Current.Resolve<IProductModelFactory>();
            List<Product> crossSellProductList = new List<Product>();


            var products = productService.GetCrossSellProductsByProductId1(productId);

            if (!products.Any())
                return Content("");

            foreach (var product in products)
            {
                Product crossSellProduct = productService.GetProductById(product.ProductId2);
                crossSellProductList.Add(crossSellProduct);
            }
            
            var model = productModelFactory.PrepareProductOverviewModels(crossSellProductList,
                productThumbPictureSize: productThumbPictureSize, forceRedirectionAfterAddingToCart: true)
                .ToList();

            return PartialView("CrossSellProducts", model);

            //return View();
        }

    }
}
