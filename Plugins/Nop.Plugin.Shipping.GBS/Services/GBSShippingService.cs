using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Shipping.GBS.Services
{
    class GBSShippingService : ShippingService
    {
        public readonly GBSShippingSetting _gbsShippingSetting;
        public GBSShippingService(IRepository<ShippingMethod> shippingMethodRepository, IRepository<Warehouse> warehouseRepository, ILogger logger, IProductService productService, IProductAttributeParser productAttributeParser, ICheckoutAttributeParser checkoutAttributeParser, IGenericAttributeService genericAttributeService, ILocalizationService localizationService, IAddressService addressService, ShippingSettings shippingSettings, IPluginFinder pluginFinder, 
            IStoreContext storeContext, IEventPublisher eventPublisher, ShoppingCartSettings shoppingCartSettings, ICacheManager cacheManager, GBSShippingSetting gbsShippingSetting) 
            : base(shippingMethodRepository, warehouseRepository, logger, productService, productAttributeParser, checkoutAttributeParser, genericAttributeService, localizationService, addressService, shippingSettings, pluginFinder, storeContext, eventPublisher, shoppingCartSettings, cacheManager)

        {
            this._gbsShippingSetting = gbsShippingSetting;
        }

        public override IList<IShippingRateComputationMethod> LoadActiveShippingRateComputationMethods(Customer customer = null, int storeId = 0)
        {
            IList<IShippingRateComputationMethod> allMethods = base.LoadActiveShippingRateComputationMethods(customer, storeId);
            IList<IShippingRateComputationMethod> gbsShippingMethod = allMethods.Where(provider => provider.PluginDescriptor.SystemName == "Nop.Plugin.Shipping.GBS").ToList();

            if (gbsShippingMethod.Any())
            {
                return gbsShippingMethod;
            }
            return allMethods;

        }

    }
}
