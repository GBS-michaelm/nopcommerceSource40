using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Checkout.GBS;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Checkout.Filters.GBS
{
    public class LoadFilters : ActionFilterAttribute, IActionFilter
    {
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        //private readonly ISpecificationAttributeService _specificationAttributeService;

        public LoadFilters(
            IStoreContext storeContext, 
            IPluginFinder pluginFinder, 
            IProductAttributeParser productAttributeParser, 
            IProductService productService, 
            ISpecificationAttributeService specificationAttributeService, 
            IWorkContext workContext,
            IOrderService orderService)
        {
            this._storeContext = storeContext;
            this._pluginFinder = pluginFinder;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._workContext = workContext;
            this._orderService = orderService;
        }
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (controllerContext.Controller is ShoppingCartController &&
                actionDescriptor.ActionName.Equals("Confirm",
                    StringComparison.InvariantCultureIgnoreCase) && (controllerContext.HttpContext.Request.HttpMethod != "POST"))
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
                         

            }
        }

    }
}
