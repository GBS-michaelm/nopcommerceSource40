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
using Nop.Plugin.BusinessLogic.GBS.Caching;
using Nop.Core.Caching;

namespace Nop.Plugin.GBSGateway.GBS.Controllers
{

    //[AdminAuthorize]

    public class GatewayPageController : BaseController
    {
        private readonly ICacheManager _lifeTimeCacheManager;

        private readonly GBSBusinessLogicSettings _gbsBusinessLogicSettings;
        public GatewayPageController (
            GBSBusinessLogicSettings gbsBusinessLogicSettings
            )
        {
            this._gbsBusinessLogicSettings = gbsBusinessLogicSettings;
            //this._lifeTimeCacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("gbs_cache_static");
            this._lifeTimeCacheManager = new GBSCacheManager();


        }

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

        [GBSOutputCache(VaryByParam = "*")]
        public ActionResult GatewayCatalogProducts(string type, int id)
        {
            switch (type)
            {
                case "sportsTeam":

                    GatewayPageProductCategoriesModel teamProductBlocksFinal = new GatewayPageProductCategoriesModel();
                    var iProductService = EngineContext.Current.Resolve<IProductService>();
                    var iCategoryService = EngineContext.Current.Resolve<ICategoryService>();
                    teamProductBlocksFinal = _lifeTimeCacheManager.Get("GatewayCatalogProductsSportsTeam" + id.ToString() + "_" + type, 60, () =>
                    {
                        GatewayPageProductCategoriesModel teamProductBlocks = new GatewayPageProductCategoriesModel();
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
                        return teamProductBlocks;
                    });
                                     
                    return View("GatewayCategoryProducts", teamProductBlocksFinal);

                default:
                    return View();
            }
            
        }

        [OutputCache(VaryByParam = "*",Duration = 60)]
        public ActionResult MarketCenterGatewayTabs(int marketCenterId, string type)
        {
            try
            {
                //ICacheManager gbsCachManager = new GBSCacheManager();
                MarketCenterGatewayTabsModel tabsContainerFinal = _lifeTimeCacheManager.Get("MarketCenterGatewayTabs_" + marketCenterId.ToString() + "_"+type, 60, () =>
                {
                    MarketCenter marketCenter = new MarketCenter(marketCenterId);
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
                    return tabsContainer;
                });
                return View("MarketCenterGatewayTabs", tabsContainerFinal);
            }          
            catch(Exception ex)
            {

                ex = new Exception("Gateway Page Controller Fail. Market Center Id: " + marketCenterId + ".");
                base.LogException(ex);

                return View();
            }

            //return View();

        }

        [GBSOutputCache(VaryByParam = "*")]
        public ActionResult GetNonMarketCenterCategories(int parentCategoryId)
        {

            MarketCenterGalleryCategoriesModel mcGallerCategoriesFinal = null;

            try
            {

                mcGallerCategoriesFinal = _lifeTimeCacheManager.Get("GetNonMarketCenterCategories_" + parentCategoryId.ToString(), 60, () =>
                {
                    MarketCenterGalleryCategoriesModel mcGallerCategories = null;
                    Company parentCategory = new Company(parentCategoryId);

                    List<CategoryModel> categories = parentCategory.GetNonMarketCenterCompanyCategories(parentCategory.id);

                    mcGallerCategories = new MarketCenterGalleryCategoriesModel();
                    mcGallerCategories.CategoriesList = categories;
                    return mcGallerCategories;

                    //return View("MarketCenterGalleryCategories", mcGallerCategories);
                });
            }
            catch (Exception ex)
            {
                ex = new Exception("Gateway Page Controller Fail. GetNonMarketCenterCategories.");
                base.LogException(ex);
            }

            return View("MarketCenterGalleryCategories", mcGallerCategoriesFinal);

        }         
        
    }
       
}
