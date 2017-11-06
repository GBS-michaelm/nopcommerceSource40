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

using Nop.Web.Factories;

namespace Nop.Plugin.Order.GBS.Factories
{
    class GBSOrderModelFactory : OrderModelFactory
    {

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
            PdfSettings pdfSettings) : base(
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
                ) { }

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public override CustomerOrderListModel PrepareCustomerOrderListModel()
        {
            CustomerOrderListModel model = base.PrepareCustomerOrderListModel();
            CustomerOrderListModel.OrderDetailsModel testorder = new CustomerOrderListModel.OrderDetailsModel();
            testorder.CustomOrderNumber = "homtest";
            testorder.OrderStatus = "archived";
            //OrderDetailsModel.OrderItemModel testitem = new OrderDetailsModel.OrderItemModel();
            //testitem.ProductId = 1;
            //testorder.
            model.Orders.Add(testorder);

            return model;
        }
    }
}
