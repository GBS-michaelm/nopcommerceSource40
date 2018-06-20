using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessLogic.GBS;
using Nop.Plugin.BusinessLogic.GBS.Domain;
using Nop.Plugin.BusinessLogic.GBS.Models;
using Nop.Services.Catalog;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.GBSGateway.GBS.Controllers
{

    //[AdminAuthorize]

    public class GatewayPageController : BaseController
    {

        private readonly GBSBusinessLogicSettings _gbsBusinessLogicSettings;
        public GatewayPageController (
            GBSBusinessLogicSettings gbsBusinessLogicSettings
            )
        {
            this._gbsBusinessLogicSettings = gbsBusinessLogicSettings;
        }

        [HttpGet]
        public IActionResult SportsTeamHtml(int id)
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
            });            
        }

        public IActionResult GatewayCatalogProducts(string type, int id)
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

        [ResponseCache(Duration = 3600, VaryByQueryKeys = new string[] { "*" })]
        public IActionResult MarketCenterGatewayTabs(int marketCenterId, string type)
        {
            try
            {
                MarketCenter marketCenter = new MarketCenter(marketCenterId);
                Dictionary<string, string> tabs = new Dictionary<string, string>();

                tabs = marketCenter.GetMarketCenterHtml(type, _gbsBusinessLogicSettings.Hack);

                //add tabs to list model with market center models inside
                MarketCenterGatewayTabsModel tabsContainer = new MarketCenterGatewayTabsModel();
                foreach (var tab in tabs)
                {
                    MarketCenterGatewayTabModel mctab = new MarketCenterGatewayTabModel();

                    if (tab.Key == "HiddenHtml")
                    {
                        tabsContainer.hiddenHtml = tab.Value;
                    }
                    else
                    {
                        mctab.tabName = tab.Key;
                        mctab.html = tab.Value;
                        tabsContainer.MarketCenterTabsList.Add(mctab);
                    }

                }

                return View("MarketCenterGatewayTabs", tabsContainer);
            }          
            catch(Exception ex)
            {

                ex = new Exception("Gateway Page Controller Fail. Market Center Id: " + marketCenterId + ".");
                base.LogException(ex);

                return View();
            }

            //return View();

        }    
        
    }
       
}
