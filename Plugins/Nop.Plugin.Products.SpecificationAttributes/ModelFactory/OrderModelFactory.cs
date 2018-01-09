using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Nop.Services.Shipping;
using NW = Nop.Web.Factories;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;

namespace Nop.Plugin.Products.SpecificationAttributes.ModelFactory
{
    public interface IOrderModelFactory : NW.IOrderModelFactory
    {
        CustomerOrderListModel PrepareCustomerOrderListModel(string status, int? page);
    }

    public partial class OrderModelFactory : NW.OrderModelFactory, IOrderModelFactory
    {
        #region Fields

        private readonly NW.IAddressModelFactory _addressModelFactory;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;

        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;

        #endregion

        #region Ctor

        public OrderModelFactory(NW.IAddressModelFactory addressModelFactory, IOrderService orderService, IWorkContext workContext, ICurrencyService currencyService, IPriceFormatter priceFormatter, IOrderProcessingService orderProcessingService, IDateTimeHelper dateTimeHelper, IPaymentService paymentService, ILocalizationService localizationService, IShippingService shippingService, ICountryService countryService, IProductAttributeParser productAttributeParser, IDownloadService downloadService, IStoreContext storeContext, IOrderTotalCalculationService orderTotalCalculationService, IRewardPointService rewardPointService, CatalogSettings catalogSettings, OrderSettings orderSettings, TaxSettings taxSettings, ShippingSettings shippingSettings, AddressSettings addressSettings, RewardPointsSettings rewardPointsSettings, PdfSettings pdfSettings) : base(addressModelFactory, orderService, workContext, currencyService, priceFormatter, orderProcessingService, dateTimeHelper, paymentService, localizationService, shippingService, countryService, productAttributeParser, downloadService, storeContext, orderTotalCalculationService, rewardPointService, catalogSettings, orderSettings, taxSettings, shippingSettings, addressSettings, rewardPointsSettings, pdfSettings)
        {
            this._addressModelFactory = addressModelFactory;
            this._orderService = orderService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;

            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public virtual CustomerOrderListModel PrepareCustomerOrderListModel(string status, int? page)
        {
            List<int> statusList = new List<int>();
            if (!string.IsNullOrEmpty(status))
            {
                if (status == ((int)OrderStatus.Cancelled).ToString())
                    statusList.Add((int)OrderStatus.Cancelled);

                if (status == ((int)OrderStatus.Pending).ToString())
                {
                    statusList.Add((int)OrderStatus.Pending);
                    statusList.Add((int)OrderStatus.Processing);
                }
            }

            var model = new CustomerOrderListModel();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageIndex: --page ?? 0, pageSize: 10, osIds: statusList);

            foreach (var order in orders)
            {
                var orderModel = new CustomerOrderListModel.OrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    OrderStatusEnum = order.OrderStatus,
                    OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                    CustomOrderNumber = order.CustomOrderNumber
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                model.Orders.Add(orderModel);
            }

            var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerOrderListModel.RecurringOrderModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)),
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialOrderId = recurringPayment.InitialOrder.Id,
                    InitialOrderNumber = recurringPayment.InitialOrder.CustomOrderNumber,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                    CanRetryLastPayment = _orderProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment)
                };

                model.RecurringOrders.Add(recurringPaymentModel);
            }

            model.CustomProperties["PagerModel"] = new PagerModel
            {
                PageSize = orders.PageSize,
                TotalRecords = orders.TotalCount,
                PageIndex = orders.PageIndex,
                ShowTotalSummary = true,
                RouteActionName = "CustomerOrdersPaged",
                UseRouteLinks = true,
                RouteValues = new OrderRouteValues { page = page ?? 0, status = status }
            };

            return model;
        }

        #endregion
    }

    public partial class OrderRouteValues : IRouteValues
    {
        public string status { get; set; }
        public int page { get; set; }
    }
}
