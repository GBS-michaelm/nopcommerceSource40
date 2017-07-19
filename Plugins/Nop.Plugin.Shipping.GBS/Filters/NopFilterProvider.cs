using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Admin.Controllers;
using CommonController = Nop.Web.Controllers.CommonController;
using Nop.Plugin.Shipping.GBS.Filters;

namespace Nop.Plugin.Shipping.GBS.Filters
{
    public class NopFilterProvider: IFilterProvider
    {
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {

            //Admin Shipping Extended fields

            if ((actionDescriptor.ControllerDescriptor.ControllerType == typeof(ProductController))
               && (actionDescriptor.ActionName.Equals("Edit"))
               && (controllerContext.HttpContext.Request.HttpMethod == "POST"))
            {
                return new[]
                    {
                        new Filter(new ExtendedFieldsAdminFilterAttribute(), FilterScope.Action, null)
                    };
            }

        
            return  new Filter[] { };
        }
    }
}