using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Checkout.GBS
{
    public class HelperClass
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        public HelperClass(IStoreContext storeContext, IWorkContext workContext)
        {

            this._storeContext = storeContext;
            this._workContext = workContext;
        }

        public Dictionary<string, object> GetNoteCardSetCount()
        {
            var spec = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            Dictionary<string, object> productSpecsDict = new Dictionary<string, object>();
            var customer = _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
              .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
              .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
              .ToList();

            foreach (var item in cart)
            {
                var specAttr = spec.GetProductSpecificationAttributes(item.ProductId);
                var option = specAttr.Select(x => x.SpecificationAttributeOption);
                var getOption = option.Where(x => x.SpecificationAttribute.DisplayOrder == 9999);
                if (getOption.Count<SpecificationAttributeOption>() > 0)
                {
                    if(!productSpecsDict.ContainsKey(item.ProductId.ToString()))
                        productSpecsDict.Add(item.ProductId.ToString(), getOption.FirstOrDefault().Name);

                }
            }

            return productSpecsDict;
        }



    }
}
