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
    public class GBSShippingService : ShippingService, IShippingService
    {
        #region Fields

        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IAddressService _addressService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shippingMethodRepository">Shipping method repository</param>
        /// <param name="warehouseRepository">Warehouse repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="productService">Product service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="addressService">Address service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="cacheManager">Cache manager</param>
        public GBSShippingService(IRepository<ShippingMethod> shippingMethodRepository,
            IRepository<Warehouse> warehouseRepository,
            ILogger logger,
            IProductService productService,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IAddressService addressService,
            ShippingSettings shippingSettings,
            IPluginFinder pluginFinder,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            ShoppingCartSettings shoppingCartSettings,
            ICacheManager cacheManager) : base(shippingMethodRepository, warehouseRepository, logger, productService, productAttributeParser, checkoutAttributeParser, genericAttributeService, localizationService,
                addressService, shippingSettings, pluginFinder, storeContext, eventPublisher, shoppingCartSettings, cacheManager) {
            this._shippingMethodRepository = shippingMethodRepository;
            this._warehouseRepository = warehouseRepository;
            this._logger = logger;
            this._productService = productService;
            this._productAttributeParser = productAttributeParser;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._addressService = addressService;
            this._shippingSettings = shippingSettings;
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._shoppingCartSettings = shoppingCartSettings;
            this._cacheManager = cacheManager;
        }

        #endregion

        public readonly GBSShippingSetting _gbsShippingSetting;

        public GBSShippingService(IRepository<ShippingMethod> shippingMethodRepository, IRepository<Warehouse> warehouseRepository, ILogger logger, IProductService productService, IProductAttributeParser productAttributeParser, ICheckoutAttributeParser checkoutAttributeParser, IGenericAttributeService genericAttributeService, ILocalizationService localizationService, IAddressService addressService, ShippingSettings shippingSettings, IPluginFinder pluginFinder,
            IStoreContext storeContext, IEventPublisher eventPublisher, ShoppingCartSettings shoppingCartSettings, ICacheManager cacheManager, GBSShippingSetting gbsShippingSetting)
            : base(shippingMethodRepository, warehouseRepository, logger, productService, productAttributeParser, checkoutAttributeParser, genericAttributeService, localizationService, addressService, shippingSettings, pluginFinder, storeContext, eventPublisher, shoppingCartSettings, cacheManager) {
            this._gbsShippingSetting = gbsShippingSetting;
        }

        public override IList<IShippingRateComputationMethod> LoadActiveShippingRateComputationMethods(Customer customer = null, int storeId = 0) {
            IWebHelper webHelper = Nop.Core.Infrastructure.EngineContext.Current.Resolve<IWebHelper>();
            var inCheckout = webHelper.GetThisPageUrl(false).ToLower().Contains("shippingaddress");

            IList<IShippingRateComputationMethod> allMethods = base.LoadActiveShippingRateComputationMethods(customer, storeId);
            IList<IShippingRateComputationMethod> gbsShippingMethod = allMethods.Where(provider => provider.PluginDescriptor.SystemName == "Nop.Plugin.Shipping.GBS").ToList();

            if (!inCheckout && gbsShippingMethod.Any())
            {
                return gbsShippingMethod;
            }

            return allMethods;
        }
    }
}
