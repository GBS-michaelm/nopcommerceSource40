using Nop.Web.Framework.Controllers;
using System.Web.Mvc;
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

        public ActionResult AccessoryCategories(int groupId)
        {
            var iCategoryService = EngineContext.Current.Resolve<ICategoryService>();

            AccessoryPageCategoryAccessoriesModel accessoryCategoryBlocks = new AccessoryPageCategoryAccessoriesModel();
            List<Accessory> accessoryList = new List<Accessory>();
            accessoryList = Accessory.GetAllCrossSellAccessories(groupId);
                        
            if(accessoryList.Count > 0)
            {
                foreach (var accessory in accessoryList)
                {

                    int price = 0;
                    IPagedList<ProductCategory> productCategoryList = iCategoryService.GetProductCategoriesByCategoryId(accessory.id);
                    var productsInOrder = productCategoryList.OrderBy(x => x.Product.Price).ToList();
                    price = (int)productsInOrder[0].Product.Price;

                    AccessoryPageBoxModel accessoryBox = new AccessoryPageBoxModel();
                    accessoryBox.name = accessory.Name;
                    accessoryBox.mainPicturePath = accessory.mainPicturePath;
                    accessoryBox.price = price;
                    accessoryBox.isFeatured = accessory.isFeatured;
                    accessoryBox.description = accessory.Description;
                    accessoryBox.featuredProductId = accessory.featuredProductId;
                    accessoryBox.categoryPageLink = accessory.SeName;

                    accessoryCategoryBlocks.accessoryBoxes.Add(accessoryBox);

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
