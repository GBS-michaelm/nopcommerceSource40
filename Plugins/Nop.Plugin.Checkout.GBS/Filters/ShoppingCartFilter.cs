using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Checkout.GBS;
using Nop.Services.Catalog;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Checkout.Filters.GBS
{
    public class ShoppingCartFilter : ActionFilterAttribute, IFilterProvider
    {
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;

        public ShoppingCartFilter(IStoreContext storeContext, IPluginFinder pluginFinder, IProductAttributeParser productAttributeParser, IProductService productService)
        {
            this._storeContext = storeContext;
            this._pluginFinder = pluginFinder;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
        }
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (controllerContext.Controller is ShoppingCartController &&
                actionDescriptor.ActionName.Equals("ProductDetails_AttributeChange",
                    StringComparison.InvariantCultureIgnoreCase)
                    && (controllerContext.HttpContext.Request.HttpMethod == "POST"))
            {
                return new List<Filter>() { new Filter(this, FilterScope.Action, 0) };
            }

            return new List<Filter>();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                int q = 1;
                JsonResult result = filterContext.Result as JsonResult;
                var json = JsonConvert.SerializeObject(result.Data);
                dynamic dresult = JsonConvert.DeserializeObject<dynamic>(json);
                NameValueCollection form = System.Web.HttpContext.Current.Request.Form;
                NameValueCollection qs = System.Web.HttpContext.Current.Request.QueryString;

                int productId = qs["productId"];

                if (System.Web.HttpContext.Current.Request.Form != null)
                {
                    foreach (string key in System.Web.HttpContext.Current.Request.Form.Keys)
                    {
                        if (key.Contains("EnteredQuantity"))
                        {
                            q = Int32.Parse(System.Web.HttpContext.Current.Request.Form[key]);
                        }
                    }
                }
                var product = _productService.GetProductById(productId);
                if (product == null)
                    return new NullJsonResult();
                string attributesXml = ParseProductAttributes(product, form);

                bool hasReturnAddressAttr = false;
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
                decimal returnAddressPriceAdjustment = 0;
                int returnAddressMinimumSurchargeID = 0;
                bool returnAddressMinimumSurchargeApplied = true;
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        hasReturnAddressAttr = (attributeValue.ProductAttributeMapping.ProductAttribute.Name == "Is Return Address" && attributeValue.Name == "Yes");
                        if (hasReturnAddressAttr)
                        {
                            returnAddressPriceAdjustment = attributeValue.PriceAdjustment;
                        }
                        if (attributeValue.ProductAttributeMapping.ProductAttribute.Name == "returnAddressMinimumSurcharge" && attributeValue.Name == "Yes")
                        {
                            returnAddressMinimumSurchargeID = attributeValue.Id;
                            returnAddressMinimumSurchargeApplied = true;
                        }
                        else
                        {
                            returnAddressMinimumSurchargeApplied = false;
                        }
                    }
                }
                if (hasReturnAddressAttr)
                {
                    int q = 1;
                    foreach (string key in form.Keys)
                    {
                        if (key.Contains("EnteredQuantity"))
                        {
                            q = Int32.Parse(System.Web.HttpContext.Current.Request.Form[key]);
                        }
                    }


                    decimal finalPrice = decimal.Parse(dresult.price.Value, NumberStyles.Currency | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);

                    if (q < 100)
                    {
                        finalPrice -= returnAddressPriceAdjustment;
                        //eventually make this configurable
                        decimal adj = (5 / (decimal)q);
                        finalPrice += adj;
                    }
                    else
                    {
                        //remove returnAddressMinimumSurcharge if still on
                        if (returnAddressMinimumSurchargeApplied)
                        {
                            //   _productAttributeParser.ParseProductAttributeMappings(attributesXml);
                        }

                    }
                    dresult.price = "$" + finalPrice;
                    //put it back in the result
                    json = JsonConvert.SerializeObject(dresult);

                    var newResult = new ContentResult()
                    {
                        Content = json,
                        ContentType = "application/json"
                    };
                    return newResult;
                }

            }
        }
    }
}
