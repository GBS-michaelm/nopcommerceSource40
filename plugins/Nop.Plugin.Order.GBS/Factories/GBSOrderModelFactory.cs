using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;
using Nop.Plugin.Order.GBS.Orders;

using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Order.GBS.Factories
{
    class GBSOrderModelFactory : OrderModelFactory
    {
        private readonly IPluginFinder _pluginFinder;

        public GBSOrderModelFactory(IAddressModelFactory addressModelFactory,
            IOrderService orderService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService,
            ILocalizationService localizationService,
            IShippingService shippingService,
            ICountryService countryService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            IStoreContext storeContext,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            PdfSettings pdfSettings,
            IPluginFinder pluginFinder) : base(
             addressModelFactory,
             orderService,
             workContext,
             currencyService,
             priceFormatter,
             orderProcessingService,
             dateTimeHelper,
             paymentService,
             localizationService,
             shippingService,
             countryService,
             productAttributeParser,
             downloadService,
             storeContext,
             orderTotalCalculationService,
             rewardPointService,
             catalogSettings,
             orderSettings,
             taxSettings,
             shippingSettings,
             addressSettings,
             rewardPointsSettings,
             pdfSettings
                ) {
            this._pluginFinder = pluginFinder;
        }

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public override CustomerOrderListModel PrepareCustomerOrderListModel()
        {
            CustomerOrderListModel model = base.PrepareCustomerOrderListModel();
           // string colmJSON = JsonConvert.SerializeObject(model, Formatting.Indented);
            var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                List<CustomerOrderListModel.OrderDetailsModel> legacyOrders = new Orders.OrderExtensions().getLegacyOrders();
                ((List<CustomerOrderListModel.OrderDetailsModel>)model.Orders).AddRange(legacyOrders);
                ((List<CustomerOrderListModel.OrderDetailsModel>)model.Orders).Sort((x, y) => y.CreatedOn.CompareTo(x.CreatedOn));
            }

            return model;
        }

        public OrderDetailsModel PrepareOrderDetailsModelLegacy(Nop.Core.Domain.Orders.Order order)
        {
            var model = new OrderDetailsModel();
            return model;
        }
    }
}
