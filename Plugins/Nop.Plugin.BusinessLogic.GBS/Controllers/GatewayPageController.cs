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
using Nop.Plugin.BusinessLogic.GBS;
using Nop.Services.Configuration;
using Nop.Web.Models.Catalog;


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

        //[HttpGet]
        //public ActionResult SportsTeamHtml(int id)
        //{           
        //    SportsTeam team = SportsTeam.GetSportsTeam(id);
        //    team.gatewayHtml = team.GenerateTeamProductHtml();


        //    GatewayPageProductBoxModel gatewayModel = new GatewayPageProductBoxModel();
        //    //gatewayModel.name = 

        //    return Json(new
        //    {
        //        success = true,
        //        h1 = team.h1,
        //        message = team.gatewayHtml
        //    }, JsonRequestBehavior.AllowGet);
            
        //}

        public ActionResult GatewayCatalogProducts(string type, int id)
        {
            switch (type)
            {
                case "sportsTeam":

                    GatewayPageProductCategoriesModel teamProductBlocks = new GatewayPageProductCategoriesModel();
                    var iProductService = EngineContext.Current.Resolve<IProductService>();
                    var iCategoryService = EngineContext.Current.Resolve<ICategoryService>();
                    IList<Category> teamCategoryProducts = iCategoryService.GetAllCategoriesByParentCategoryId(id);

                    //check if football via user setting here
                    //if one is football assume all are football for now.
                    int footballId = _gbsBusinessLogicSettings.SportsFootballDefaultId;
                    bool isFootball = false;
                    foreach (Category cat in teamCategoryProducts)
                    {
                        if (!isFootball)
                        {
                            int curParentCategoryId = cat.ParentCategoryId;

                            while (curParentCategoryId != 0)
                            {
                                if (curParentCategoryId == footballId)
                                {
                                    isFootball = true;
                                    break;
                                }
                                else
                                {
                                    Category higherCat = iCategoryService.GetCategoryById(curParentCategoryId);
                                    curParentCategoryId = higherCat.ParentCategoryId;
                                }

                            }
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    

                    foreach (var category in teamCategoryProducts)
                    {
                        
                        SportsTeam customExtendedCategoryData = SportsTeam.GetSportsTeam(category.Id);
                        IPagedList<ProductCategory> productCategoryList = iCategoryService.GetProductCategoriesByCategoryId(category.Id);
                        Product featuredProductData = iProductService.GetProductById(customExtendedCategoryData.FeaturedProductId);

                        GatewayPageProductBoxModel productBox = new GatewayPageProductBoxModel();
                        productBox.name = category.Name;
                        productBox.mainPicturePath = isFootball ? "" : customExtendedCategoryData.MainPicturePath;
                        productBox.width = featuredProductData.Width;
                        productBox.length = featuredProductData.Length;
                        productBox.designCount = productCategoryList.Count;
                        productBox.price = (int)featuredProductData.Price;
                        productBox.isFeatured = customExtendedCategoryData.IsFeatured;
                        productBox.featuredProductId = customExtendedCategoryData.FeaturedProductId;
                        productBox.productLink = customExtendedCategoryData.SeName;

                        teamProductBlocks.productBoxes.Add(productBox);

                    }
                                     
                    return View("GatewayCategoryProducts", teamProductBlocks);

                default:
                    return View();
            }
            
        }

        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult SportsTeamGatewayTabs(int sportCategoryId)
        {

            try
            {
                SportsTabs sportsTabs = SportsTabs.GetSportsTabs(sportCategoryId);
                return View("SportsTabs", sportsTabs);
            }
            catch(Exception ex)
            {

                ex = new Exception("Gateway Page Controller Fail. SportsTeamGatewayTabs.");
                base.LogException(ex);
                return View();
            }
                        
        }
        
        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult MarketCenterGatewayTabs(int marketCenterId, string type)
        {
            try
            {
                MarketCenter marketCenter = MarketCenter.GetMarketCenter(marketCenterId);
                Dictionary<string, string> tabs = new Dictionary<string, string>();

                tabs = marketCenter.GetMarketCenterHtml(type, _gbsBusinessLogicSettings.Hack);

                //add tabs to list model with market center models inside
                MarketCenterGatewayTabsModel tabsContainer = new MarketCenterGatewayTabsModel();
                foreach (var tab in tabs)
                {
                    MarketCenterGatewayTabModel mctab = new MarketCenterGatewayTabModel();

                    if (tab.Key == "HiddenChildrenHtml")
                    {
                        tabsContainer.hiddenHtml = tab.Value;
                    }
                    else if (tab.Key == "HiddenAll")
                    {
                        tabsContainer.hiddenAll = tab.Value;
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

        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult GetNonMarketCenterCategories(int parentCategoryId)
        {

            MarketCenterGalleryCategoriesModel mcGalleryCategories = null;

            try
            {
                Company parentCompany = Company.GetCompany(parentCategoryId);

                List<Company> categories = parentCompany.GetNonMarketCenterCompanyCategories(parentCompany);

                mcGalleryCategories = new MarketCenterGalleryCategoriesModel();
                mcGalleryCategories.CategoriesList = categories;

                //return View("MarketCenterGalleryCategories", mcGallerCategories);
            }
            catch (Exception ex)
            {
                ex = new Exception("Gateway Page Controller Fail. GetNonMarketCenterCategories.");
                base.LogException(ex);
            }

            return View("MarketCenterGalleryCategories", mcGalleryCategories);

        }         
        
    }
       
}
