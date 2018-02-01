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
                        productBox.productLink = customExtendedCategoryData.SeName;

                        teamProductBlocks.productBoxes.Add(productBox);

                    }
                                     
                    return View("GatewayCategoryProducts", teamProductBlocks);

                default:
                    return View();
            }
            
        }


        public ActionResult MarketCenterTabs(int marketCenterId)
        {

            MarketCenter marketCenter = new MarketCenter(marketCenterId);
            Dictionary<string, string> tabs = new Dictionary<string, string>();

            tabs = marketCenter.GetMarketCenterHtml();

            //add tabs to list model with market center models inside

            return View();

        }


        
    }

    

}
