using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Discounts;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Plugins;
using Nop.Plugin.PriceCalculation.DataAccess.GBS;
using System.Data;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.PriceCalculation.GBS.Catalog
{
    class GBSPriceCalculationService : PriceCalculationService
    {

        ICacheManager cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;


        #region Ctor

        public GBSPriceCalculationService(IPluginFinder pluginFinder, IWorkContext workContext, IStoreContext storeContext, IDiscountService discountService, ICategoryService categoryService, IManufacturerService manufacturerService, IProductAttributeParser productAttributeParser, IProductService productService, ICacheManager cacheManager, ShoppingCartSettings shoppingCartSettings, CatalogSettings catalogSettings)
            : base(workContext, storeContext, discountService, categoryService, manufacturerService, productAttributeParser, productService, cacheManager, shoppingCartSettings, catalogSettings)
        {
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;

        }
        #endregion

        public override decimal GetUnitPrice(Product product,
            Customer customer,
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate, DateTime? rentalEndDate,
            bool includeDiscounts,
            out decimal discountAmount,
            out List<DiscountForCaching> appliedDiscounts)
        {  
            var miscPlugins = _pluginFinder.GetPlugins<MyPriceCalculationServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                #region Amalgamation

                bool useCategoryAmalgamation = false;

                //AMALGAMTION is only for products that use cartons at this point
                DBManager manager = new DBManager();
                ICategoryService iCategoryService = EngineContext.Current.Resolve<ICategoryService>();
                ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
                //check if amalgamation is on
                //usp check amalgamation

                IList<ProductCategory> productCategories = iCategoryService.GetProductCategoriesByProductId(product.Id);
                //string categoryIds = "";
                //for (int i = 0; i < productCategories.Count; i++)
                //{                    
                //    if(i < productCategories.Count && i != 0)
                //    {
                //         categoryIds += "," + productCategories[i].CategoryId.ToString();
                //    }else
                //    {
                //        categoryIds += productCategories[i].CategoryId.ToString();
                //    }
                //}
                var preCheckSpecAttrs = specService.GetProductSpecificationAttributes(product.Id);
                int whatId = 0;
                foreach (var spec in preCheckSpecAttrs)
                {
                    if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "WhatAmI")
                    {
                        whatId = spec.SpecificationAttributeOption.Id;
                    }
                }

                DataView amalgamationCheckDataView = cacheManager.Get("checkSpecAttrAmalgamation" + product.Id, 60, () => {

                    string whichAmalgamationToUseQuery = "EXEC usp_SelectGBSAmalgamationFeaturedIds  @specificationAttributeOptionId";
                    Dictionary<string, Object> amalgamationCheckDic = new Dictionary<string, Object>();
                    amalgamationCheckDic.Add("@specificationAttributeOptionId", whatId);
                    DataView innerAmalgamationCheckDataView = manager.GetParameterizedDataView(whichAmalgamationToUseQuery, amalgamationCheckDic);

                    return innerAmalgamationCheckDataView;

                });
                
                //useCategoryAmalgamation = amalgamationCheckDataView.Count > 0 ? false : true;
                
                #region categoryAmalgamation
                //Original category version of amalgmation

                if (useCategoryAmalgamation)
                {
                    List<int> categoryIdsList = new List<int>();
                    foreach (var category in productCategories)
                    {
                        categoryIdsList.Add(category.CategoryId);
                    }

                    //eventually current packtype will need to be an attribute passed in
                    string curProductPackType = "";
                    var curItemSpecAttrs = specService.GetProductSpecificationAttributes(product.Id);
                    foreach (var spec in curItemSpecAttrs)
                    {
                        if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                        {
                            curProductPackType = spec.SpecificationAttributeOption.Name;
                        }
                    }

                    string amalgamationDataQuery = "EXEC usp_SelectGBSAmalgamationMaster @categoryId";
                    Dictionary<string, object> amalgamationDic = new Dictionary<string, object>();
                    amalgamationDic.Add("@CategoryId", "");

                    foreach (var cat in categoryIdsList)
                    {
                        amalgamationDic["@CategoryId"] = cat;

                        DataView amalgamationDataView = cacheManager.Get("marketCenterChildCompany" + cat, 60, () => {

                            DataView innerAmalgamationDataView = manager.GetParameterizedDataView(amalgamationDataQuery, amalgamationDic);

                            return innerAmalgamationDataView;

                        });
                        
                        if (amalgamationDataView != null && amalgamationDataView.Count > 0 && curProductPackType == "Carton")
                        {
                            ICollection<ShoppingCartItem> cartItemList = customer.ShoppingCartItems;
                            if (cartItemList != null)
                            {
                                List<int> amalgamationMasterCategoryList = new List<int>(); //used if multiple master category id are returned
                                int masterCategoryId; //used to get featured product id
                                int amalgamationGroupId; //group that holds all the associated category ids
                                int bestPriceProductId;
                                int qty = 0;

                                for (int i = 0; i < amalgamationDataView.Count; i++)
                                {
                                    //add all return master Ids 
                                    amalgamationMasterCategoryList.Add(Int32.Parse(amalgamationDataView[i]["masterCategoryId"].ToString()));
                                }

                                int[] masterIdProductIdGroupId = GetRealMasterId(amalgamationMasterCategoryList);
                                masterCategoryId = masterIdProductIdGroupId[0];
                                bestPriceProductId = masterIdProductIdGroupId[1];
                                amalgamationGroupId = masterIdProductIdGroupId[2];

                                List<int> categoryGroupMembersIds = GetCategoryGroupIds(amalgamationGroupId);

                                Dictionary<int, int> qtyEachDic = new Dictionary<int, int>();

                                //remove items that don't use the carton packtype
                                List<ShoppingCartItem> AmalgamationList = new List<ShoppingCartItem>();
                                foreach (var item in cartItemList)
                                {
                                    var specAttrs = specService.GetProductSpecificationAttributes(item.ProductId);
                                    foreach (var spec in specAttrs)
                                    {
                                        string type = "";
                                        if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                                        {
                                            type = spec.SpecificationAttributeOption.Name;
                                            if (type == "Carton")
                                            {
                                                //cartItemList.Remove(item);
                                                AmalgamationList.Add(item);
                                            }
                                        }
                                    }
                                }                              

                                for (int i = 0; i < categoryGroupMembersIds.Count; i++)
                                {
                                    foreach (ShoppingCartItem item in AmalgamationList)
                                    {
                                        IList<ProductCategory> cartProductCategories = iCategoryService.GetProductCategoriesByProductId(item.ProductId);

                                        foreach (ProductCategory cartCategory in cartProductCategories)
                                        {
                                            if (cartCategory.CategoryId == categoryGroupMembersIds[i])
                                            {
                                                if (!qtyEachDic.ContainsKey(item.Id))
                                                {
                                                    qtyEachDic.Add(item.Id, item.Quantity);
                                                }

                                            }
                                        }

                                    }
                                }

                                foreach (KeyValuePair<int, int> pair in qtyEachDic)
                                {
                                    qty += pair.Value;
                                }

                                //qty = 20;

                                product = _productService.GetProductById(bestPriceProductId);
                                quantity = qty;
                            }

                            break;

                        }

                    }
                    
                    //get quantity by finding the number of items that belong to the same alamgamation category
                    //change quantity on get unit price to get proper per unit price.
                    //if (product.Id == 4430)
                    //{
                    //    quantity = 31;
                    //}
                }

                #endregion categoryAmalgamation

                #region specAttrAmalgamation
                //spec attr amalgamtion
                else
                {
                    //Get product categories
                    //List<int> categoryIdsList = new List<int>();
                    //foreach (var category in productCategories)
                    //{
                    //    categoryIdsList.Add(category.CategoryId);
                    //}

                    //get product whatamI and pack type
                    string curProductPackType = "";
                    int curProductWhatAmIId = 0;
                    int featuredProductId = 0;
                    var curItemSpecAttrs = specService.GetProductSpecificationAttributes(product.Id);
                    ICollection<ShoppingCartItem> cartItemList = customer.ShoppingCartItems;
                    foreach (var spec in curItemSpecAttrs)
                    {
                        if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                        {
                            curProductPackType = spec.SpecificationAttributeOption.Name;
                        }
                        if(spec.SpecificationAttributeOption.SpecificationAttribute.Name == "WhatAmI")
                        {
                            curProductWhatAmIId = spec.SpecificationAttributeOption.Id;
                        }
                    }

                    //use product whatami and categories to check if amalgamation is on, via function
                    featuredProductId = curProductWhatAmIId != 0 ? CheckAmalgamation(curProductWhatAmIId, product.Id) : 0; //categoryIdsList, 
                    //return featured productid
                                                            
                    //do amalgamation checks, else just use regular cart price calculation
                    if(featuredProductId != 0 && curProductPackType == "Carton")
                    {
                        //amalgamation
                        //check all of cart items whatamI and pack type
                        //for each that matches current amalgamation whatamI and pack type get qty of item
                        quantity = AmalgamationProductsQuantity(cartItemList, curProductWhatAmIId);
                        //get featured product via featured productid 
                        //set product and qty for base to use in price calculation
                        product = _productService.GetProductById(featuredProductId);

                    }
                    
                }
                #endregion specAttrAmalgamation


                #endregion Amalgamation

                decimal finalPrice = base.GetUnitPrice(product, customer, shoppingCartType, quantity, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, includeDiscounts, out discountAmount, out appliedDiscounts);
                //eventually make this a configurable rules plugin
                bool hasReturnAddressAttr = false;
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
                decimal returnAddressPriceAdjustment = 0;
                //int returnAddressMinimumSurchargeID = 0;
                //bool returnAddressMinimumSurchargeApplied = true;
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        if (attributeValue.ProductAttributeMapping.ProductAttribute.Name == "Add Return Address" && attributeValue.Name == "Yes")
                        {
                            hasReturnAddressAttr = true;
                            returnAddressPriceAdjustment = attributeValue.PriceAdjustment;

                        }

                        //if (attributeValue.ProductAttributeMapping.ProductAttribute.Name == "returnAddressMinimumSurcharge" && attributeValue.Name == "Yes")
                        //{
                        //    returnAddressMinimumSurchargeID = attributeValue.Id;
                        //    returnAddressMinimumSurchargeApplied = true;
                        //}else
                        //{
                        //    returnAddressMinimumSurchargeApplied = false;
                        //}
                    }
                }
                if (hasReturnAddressAttr)
                {
                    int q = quantity;
                    //quanitity is being set to 1 for product details and Customers Canvas Editor in NOP Core, so have to get it from th Form
                    if (System.Web.HttpContext.Current.Request.Form != null) {
                        foreach (string key in System.Web.HttpContext.Current.Request.Form.Keys)
                        {
                            if (key.Contains("EnteredQuantity")){
                                q = Int32.Parse(System.Web.HttpContext.Current.Request.Form[key]);
                            }
                        }
                    }

                    if (q < 100)    //quantity is being passed from CC correctly?
                    {
                        finalPrice -= returnAddressPriceAdjustment;
                        //eventually make this configurable
                        decimal adj = (5 / (decimal)q);
                        finalPrice += adj;
                    }
                    else
                    {
                        ////remove returnAddressMinimumSurcharge if still on
                        //if (returnAddressMinimumSurchargeApplied)
                        //{
                        //    //   _productAttributeParser.ParseProductAttributeMappings(attributesXml);
                        //}

                    }

                }
                return finalPrice;
            }

            //if we get here it is because the plugin isn't installed and the base class is called
            return base.GetUnitPrice(product, customer, shoppingCartType, quantity, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, includeDiscounts, out discountAmount, out appliedDiscounts);

        }

        //for multiple master ids
        private int[] GetRealMasterId(List<int> list)
        {
            DBManager manager = new DBManager();
            int[] masterIdProductIdGroupId = new int[3];
            int lowestPricedProductID = 00;
            int masterCategoryID = 00; //needed to get group number later
            int groupId = 00;
            decimal currentLowestPrice = 999999.01M;

            for (int i = 0; i < list.Count; i++)
            {
                //query for featured product id
                string featuredProductDataQuery = "EXEC usp_SelectGBSFeaturedProductAndGroup @categoryId";
                Dictionary<string, string> featuredProductDic = new Dictionary<string, string>();
                featuredProductDic.Add("@CategoryId", list[i].ToString());
                DataView featuredProductDataView = manager.GetParameterizedDataView(featuredProductDataQuery, featuredProductDic);
                
                if(featuredProductDataView.Count > 0)
                {
                    Product featured = _productService.GetProductById(Int32.Parse(featuredProductDataView[i]["FeaturedProductId"].ToString()));
                    
                    if(i != 0) //init pass, just set values
                    {
                        if(featured.Price < currentLowestPrice)
                        {
                            lowestPricedProductID = featured.Id;
                            masterCategoryID = list[i];
                            currentLowestPrice = featured.Price;
                            groupId = Int32.Parse(featuredProductDataView[i]["amalgamationGroupId"].ToString());
                        }

                    }else
                    {
                        lowestPricedProductID = featured.Id;
                        masterCategoryID = list[i];
                        currentLowestPrice = featured.Price;
                        groupId = Int32.Parse(featuredProductDataView[i]["amalgamationGroupId"].ToString());
                    }
                    
                }

            }

            masterIdProductIdGroupId[0] = masterCategoryID;
            masterIdProductIdGroupId[1] = lowestPricedProductID;
            masterIdProductIdGroupId[2] = groupId;
            
            return masterIdProductIdGroupId;
        }
        
        private List<int> GetCategoryGroupIds(int groupId)
        {
            DBManager manager = new DBManager();
            List<int> categoryGroupMemberIdsList = new List<int>();


            DataView categoryGroupMembersDataView = cacheManager.Get("categoryGroupMembersAmalgamation" + groupId.ToString(), 60, () => {

            string groupIdDataQuery = "EXEC usp_SelectGBSAllCategoryGroupMembers @amalgamationGroupId";
            Dictionary<string, Object> categoryGroupMembersDic = new Dictionary<string, Object>();
            categoryGroupMembersDic.Add("@amalgamationGroupId", groupId);
            DataView innerCategoryGroupMembersDataView = manager.GetParameterizedDataView(groupIdDataQuery, categoryGroupMembersDic);

                return innerCategoryGroupMembersDataView;

            });

            if (categoryGroupMembersDataView.Count > 0)
            {

                for (int i = 0; i < categoryGroupMembersDataView.Count; i++)
                {
                    categoryGroupMemberIdsList.Add(Int32.Parse(categoryGroupMembersDataView[i]["categoryId"].ToString()));
                }
                
            }
            
            return categoryGroupMemberIdsList;

        }

        #region AmalgamtionSpecAttrFunctions
        private int CheckAmalgamation(int whatAmIId, int productId) //List<int> categoryIds,
        {
            DBManager manager = new DBManager();
            List<int> featuredIdsList = new List<int>();
            int featuredProductID = 0;

            string amalgamationCheck = "EXEC usp_SelectGBSAmalgamationFeaturedIds  @specificationAttributeOptionId"; //@CategoryId,

            //foreach product category query to see if amalgamation is on
            //foreach (var categoryId in categoryIds)
            //{
            
            DataView amalgamationCheckDataView = cacheManager.Get("checkSpecAttrAmalgamation" + productId.ToString(), 60, () => {
                
                Dictionary<string, Object> amalgamationCheckDic = new Dictionary<string, Object>();
                //amalgamationCheckDic.Add("@CategoryId", categoryId);
                amalgamationCheckDic.Add("@specificationAttributeOptionId", whatAmIId);

                DataView innerAmalgamationCheckDataView = manager.GetParameterizedDataView(amalgamationCheck, amalgamationCheckDic);
                //if is on add that category data(catid, featuredid) to a list

                return innerAmalgamationCheckDataView;

            });

            if (amalgamationCheckDataView.Count > 0)
                {
                    for (int i = 0; i < amalgamationCheckDataView.Count; i++)
                    {
                        featuredIdsList.Add(Int32.Parse(amalgamationCheckDataView[i]["FeaturedProductId"].ToString()));
                    }                  
                }

            //}

            //list will usually only have 1 member
            //in the event of two a comparision between the featured ids price must be made
            if (featuredIdsList.Count > 1)
            {
                featuredProductID = GetRealFeaturedId(featuredIdsList);
            }
            else
            {
                if (featuredIdsList.Count > 0)
                {
                    featuredProductID = featuredIdsList[0];
                }
            }

            return featuredProductID;
                        
        }

        private int GetRealFeaturedId(List<int> featuredProductIds)
        {
            DBManager manager = new DBManager();
            int realFeaturedId = 0;
            decimal currentLowestPrice = 999999.01M;

            for (int i = 0; i < featuredProductIds.Count; i++)
            {
                Product featured = _productService.GetProductById(featuredProductIds[i]);

                if (i != 0) //init pass, just set values
                {
                    if (featured.Price < currentLowestPrice)
                    {
                        realFeaturedId = featured.Id;
                        currentLowestPrice = featured.Price;
                    }

                }
                else
                {
                    realFeaturedId = featured.Id;
                    currentLowestPrice = featured.Price;
                }
                
            }
            
            return realFeaturedId;
        }

        private int AmalgamationProductsQuantity(ICollection<ShoppingCartItem> cartItemList, int whatAmIId)
        {
            ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            Dictionary<int, int> qtyEachDic = new Dictionary<int, int>();
            int qty = 0;

            //remove items that don't use the carton packtype
            List<ShoppingCartItem> AmalgamationList = new List<ShoppingCartItem>();
            foreach (var item in cartItemList)
            {
                var specAttrs = specService.GetProductSpecificationAttributes(item.ProductId);
                string type = "";
                int whatAmI = 0;
                foreach (var spec in specAttrs)
                {                   
                    if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "Pack Type")
                    {
                        type = spec.SpecificationAttributeOption.Name;                      
                    }
                    if (spec.SpecificationAttributeOption.SpecificationAttribute.Name == "WhatAmI")
                    {
                        whatAmI = spec.SpecificationAttributeOption.Id;
                    }
                }

                if (type == "Carton" && whatAmI == whatAmIId)
                {
                    AmalgamationList.Add(item);
                }
                
            }

            foreach (ShoppingCartItem item in AmalgamationList)
            {
                if (!qtyEachDic.ContainsKey(item.Id))
                {
                    qtyEachDic.Add(item.Id, item.Quantity);
                }
            }

            foreach (KeyValuePair<int, int> pair in qtyEachDic)
            {
                qty += pair.Value;
            }
            
            return qty;
            
        }
                
        #endregion AmalgamtionSpecAttrFunctions

    }


}
