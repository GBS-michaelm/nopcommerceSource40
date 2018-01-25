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
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.GBSGateway.GBS.Controllers
{

    //[AdminAuthorize]

    public class GatewayPageController : BaseController
    {

        
        [HttpGet]
        public ActionResult SportsTeamHtml(int id)
        {           
            SportsTeam team = new SportsTeam(id);
            team.gatewayHtml = team.GenerateTeamProductHtml();


            GatewayPageProductBoxModel gatewayModel = new GatewayPageProductBoxModel();
            //gatewayModel.name = 

            return Json(new
            {
                success = true,
                h1 = team.h1,
                message = team.gatewayHtml
            }, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult GatewayCatalogProducts(string type, int id)
        {
            switch (type)
            {
                case "sportsTeam":

                    GatewayPageProductCategoriesModel teamProductBlocks = new GatewayPageProductCategoriesModel();
                    var iProductService = EngineContext.Current.Resolve<IProductService>();
                    var iCategoryService = EngineContext.Current.Resolve<ICategoryService>();
                    IList<Category> teamCategoryProducts = iCategoryService.GetAllCategoriesByParentCategoryId(id);

                    
                    foreach (var category in teamCategoryProducts)
                    {
                        
                        SportsTeam customExtendedCategoryData = new SportsTeam(category.Id);
                        IPagedList<ProductCategory> productCategoryList = iCategoryService.GetProductCategoriesByCategoryId(category.Id);
                        Product featuredProductData = iProductService.GetProductById(customExtendedCategoryData.featuredProductId);

                        GatewayPageProductBoxModel productBox = new GatewayPageProductBoxModel();
                        productBox.name = category.Name;
                        productBox.mainPicturePath = customExtendedCategoryData.mainPicturePath;
                        productBox.width = featuredProductData.Width;
                        productBox.length = featuredProductData.Length;
                        productBox.designCount = productCategoryList.Count;
                        productBox.price = (int)featuredProductData.Price;
                        productBox.isFeatured = customExtendedCategoryData.isFeatured;
                        productBox.featuredProductId = customExtendedCategoryData.featuredProductId;
                        //if category product count == 1 then sete link to productpage and not gallery page
                        string correctUrl = LonerUrlHandle(productCategoryList, customExtendedCategoryData);
                        productBox.productLink = correctUrl;
                        //productBox.productLink = customExtendedCategoryData.SeName;

                        teamProductBlocks.productBoxes.Add(productBox);

                    }
                                     
                    return View("GatewayCategoryProducts", teamProductBlocks);

                default:
                    return View();
            }
            
        }

        //Handle for categories that only have one product 
        private string LonerUrlHandle(IPagedList<ProductCategory> productCategoryList, SportsTeam customExtendedCategoryData)
        {
            string correctUrl = "";
            bool moreThanTwoGroups = false;
            bool groupIdSame = true;
            int groupedProductGroupsCount = 0;
            int groupId = 0;
            bool noParentCountGreaterThanTwoProducts = false;
      
            foreach (var product in productCategoryList)
            {
                if (product.Product.ProductType == ProductType.GroupedProduct)
                {
                    groupedProductGroupsCount++;

                    if (groupedProductGroupsCount > 1)
                    {
                        moreThanTwoGroups = true;
                    }
                }
                //0 is the id for the parent group of the parent group itself, which doesn't exist because it is already the parent group
                if (product.Product.ParentGroupedProductId != 0)
                {
                    if (groupId == 0)
                    {
                        groupId = product.Product.ParentGroupedProductId;
                        
                    }
                    else
                    {
                        //different group ids mean multiple group products and thus mutiple groups
                        if (groupId != product.Product.ParentGroupedProductId)
                        {
                            groupIdSame = false;
                        }
                    }
                }
                
            }
            //no parents, so no groups, and count of products greater than 1
            if (groupId == 0 && groupedProductGroupsCount == 0 && productCategoryList.Count > 1)
            {
                noParentCountGreaterThanTwoProducts = true;
            }
            
            //gallery url
            if(moreThanTwoGroups == true || groupIdSame == false || noParentCountGreaterThanTwoProducts == true)
            {
                correctUrl = customExtendedCategoryData.SeName;
            }
            else
            {
                //product page url
                IProductService iProductService = EngineContext.Current.Resolve<IProductService>();
                IProductModelFactory productModelFactory = EngineContext.Current.Resolve<IProductModelFactory>();

                Product product = iProductService.GetProductById(groupId);
                Product[] prodArray = { product };               
                IEnumerable<Product> products = prodArray;

                IEnumerable<ProductOverviewModel> productOverviewModel = productModelFactory.PrepareProductOverviewModels(products);
                List<ProductOverviewModel> list = (List <ProductOverviewModel>)productOverviewModel;

                correctUrl = list[0].SeName;         

            }

            return correctUrl;
        }
        
    }   

}
