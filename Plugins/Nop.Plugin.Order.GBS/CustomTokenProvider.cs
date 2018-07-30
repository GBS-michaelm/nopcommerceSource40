using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Messages;
using Nop.Core.Plugins;
using Nop.Web.Framework;
using Nop.Core.Domain;
using Nop.Core.Domain.Payments;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Order.GBS
{
    public class CustomTokenProvider : MessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly IDownloadService _downloadService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;

        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly PaymentSettings _paymentSettings;

        private readonly IEventPublisher _eventPublisher;
        private readonly IPluginFinder _pluginFinder;
        private readonly StoreInformationSettings _storeInformationSettings;
        #endregion

        #region Ctor

        public CustomTokenProvider(
            ILanguageService languageService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            ICurrencyService currencyService,
            IWorkContext workContext,
            IDownloadService downloadService,
            IOrderService orderService,
            IPaymentService paymentService,
            IStoreService storeService,
            IStoreContext storeContext,
            IProductAttributeParser productAttributeParser,
            IAddressAttributeFormatter addressAttributeFormatter,
            ICustomerAttributeFormatter customerAttributeFormatter,
			IUrlHelperFactory urlHelperFactory,
			IActionContextAccessor actionContextAccessor,
			MessageTemplatesSettings templatesSettings,
            CatalogSettings catalogSettings,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            ShippingSettings shippingSettings,
            PaymentSettings paymentSettings,
            IEventPublisher eventPublisher,
            StoreInformationSettings storeInformationSettings,
            IPluginFinder pluginFinder) 
            : 
            base(languageService,
                localizationService,
                dateTimeHelper,
                priceFormatter,
                currencyService,
                workContext,
                downloadService,
                orderService,
                paymentService,
                storeService,
                storeContext,
                productAttributeParser,
                addressAttributeFormatter,
                customerAttributeFormatter,  
				urlHelperFactory,
				actionContextAccessor,
                templatesSettings,
                catalogSettings,
                taxSettings,
                currencySettings,
                shippingSettings,
                paymentSettings,
                eventPublisher,
                storeInformationSettings)
        {
            
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._workContext = workContext;
            this._downloadService = downloadService;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._productAttributeParser = productAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._customerAttributeFormatter = customerAttributeFormatter;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._templatesSettings = templatesSettings;
            this._catalogSettings = catalogSettings;
            this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
            this._shippingSettings = shippingSettings;
            this._paymentSettings = paymentSettings;
            this._eventPublisher = eventPublisher;
            this._storeInformationSettings = storeInformationSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion

        public override void AddOrderTokens(IList<Token> tokens, Nop.Core.Domain.Orders.Order order, int languageId, int vendorId = 0)
        {
            var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: order.StoreId).ToList();
            if (miscPlugins.Count > 0)
            {
				//string orderID = BuildOrderID(order, languageId);
				NopResourceDisplayNameAttribute orderNumberKeyGBS = new NopResourceDisplayNameAttribute("Account.CustomerOrders.OrderNumber");

                tokens.Add(new Token("Order.GBSOrderID", order.DeserializeCustomValues()[orderNumberKeyGBS.DisplayName].ToString(), true));

                base.AddOrderTokens(tokens, order, languageId);
            }
        }

    }
}
