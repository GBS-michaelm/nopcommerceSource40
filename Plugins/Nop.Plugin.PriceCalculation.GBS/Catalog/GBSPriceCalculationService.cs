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

namespace Nop.Plugin.PriceCalculation.GBS.Catalog
{
    class GBSPriceCalculationService : PriceCalculationService
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;
        private readonly IProductAttributeParser _productAttributeParser;


        #region Ctor

        public GBSPriceCalculationService(IPluginFinder pluginFinder, IWorkContext workContext, IStoreContext storeContext, IDiscountService discountService, ICategoryService categoryService, IManufacturerService manufacturerService, IProductAttributeParser productAttributeParser, IProductService productService, ICacheManager cacheManager, ShoppingCartSettings shoppingCartSettings, CatalogSettings catalogSettings)
            : base(workContext, storeContext, discountService, categoryService, manufacturerService, productAttributeParser, productService, cacheManager, shoppingCartSettings, catalogSettings)
        {
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
            this._productAttributeParser = productAttributeParser;


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


    }
}
