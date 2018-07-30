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
using NW = Nop.Web.Factories;
using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Services.Logging;
using System.Web;

namespace Nop.Plugin.Order.GBS.Factories
{
    public interface IOrderModelFactory : NW.IOrderModelFactory
    {
        CustomerOrderListModel PrepareCustomerOrderListModel(string status, int? page, int? pageSize);
    }

    public class GBSOrderModelFactory : OrderModelFactory, IOrderModelFactory
    {
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

        private readonly IPluginFinder _pluginFinder;
        private readonly GBSOrderSettings _gbsOrderSettings;
        private readonly ILogger _logger;

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
            IPluginFinder pluginFinder,
            GBSOrderSettings gbsOrderSettings,
            ILogger logger) : base(
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
                )
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

            this._pluginFinder = pluginFinder;
            this._gbsOrderSettings = gbsOrderSettings;
            this._logger = logger;

        }

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public virtual CustomerOrderListModel PrepareCustomerOrderListModel(string status, int? page, int? pageSize)
        {
            if (page == 0)
                page = null;

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
                customerId: _workContext.CurrentCustomer.Id, osIds: statusList); //pageIndex: --page ?? 0, pageSize: 5,

            List<CustomerOrderListModel.OrderDetailsModel> allOrders = new List<CustomerOrderListModel.OrderDetailsModel>();
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

                allOrders.Add(orderModel);
            }

            List<CustomerOrderListModel.OrderDetailsModel> legacyOrders = null;
            var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0 && _gbsOrderSettings.LegacyOrdersInOrderHistory)
            {
                legacyOrders = new Orders.OrderExtensions().getLegacyOrders();
                if (!string.IsNullOrEmpty(status))
                {
                    legacyOrders = legacyOrders.Where(x => statusList.Contains((int)x.OrderStatusEnum)).ToList();
                }

                if (legacyOrders != null && legacyOrders.Count() > 0)
                {
                    allOrders.AddRange(legacyOrders);
                }
                allOrders.Sort((x, y) => y.CreatedOn.CompareTo(x.CreatedOn));
            }

            var ordersPaging = new PagedList<CustomerOrderListModel.OrderDetailsModel>(allOrders, pageIndex: --page ?? 0, pageSize: pageSize ?? 5);
            model.Orders = ordersPaging.ToList();

            // do paging on orders

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
                PageSize = ordersPaging.PageSize,
                TotalRecords = ordersPaging.TotalCount,
                PageIndex = ordersPaging.PageIndex,
                ShowTotalSummary = true,
                RouteActionName = "CustomerOrders",
                UseRouteLinks = true,
                RouteValues = new OrderRouteValues { page = page ?? 0, status = status, pageSize = pageSize}
            };

            if (model.Orders.Any())
            {
                model.Orders.FirstOrDefault().CustomProperties["PagerModel"] = new PagerModel
                {
                    PageSize = ordersPaging.PageSize,
                    TotalRecords = ordersPaging.TotalCount,
                    PageIndex = ordersPaging.PageIndex,
                    ShowTotalSummary = true,
                    RouteActionName = "CustomerOrders",
                    UseRouteLinks = true,
                    RouteValues = new OrderRouteValues { page = page ?? 0, status = status, pageSize = pageSize }
                };
            }

            return model;
        }

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public override CustomerOrderListModel PrepareCustomerOrderListModel()
        {
            //_logger.Information("Entered PrepareCustomerOrderListModel");

            CustomerOrderListModel model = new CustomerOrderListModel();
            // string colmJSON = JsonConvert.SerializeObject(model, Formatting.Indented);
            List<CustomerOrderListModel.OrderDetailsModel> legacyOrders = null;
            try
            {
                model = base.PrepareCustomerOrderListModel();
                var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0 && _gbsOrderSettings.LegacyOrdersInOrderHistory)
                {
                    legacyOrders = new Orders.OrderExtensions().getLegacyOrders();
                    //_logger.Information("Trace - legacyOrders.count = " + (legacyOrders != null ? Convert.ToString(legacyOrders.Count()) : "NULL"));

                    if (legacyOrders != null && legacyOrders.Count() > 0)
                    {
                        ((List<CustomerOrderListModel.OrderDetailsModel>)model.Orders).AddRange(legacyOrders);
                        //_logger.Information("added range to orders");

                    }
                    ((List<CustomerOrderListModel.OrderDetailsModel>)model.Orders).Sort((x, y) => y.CreatedOn.CompareTo(x.CreatedOn));
                    //_logger.Information("sorted orders");

                }
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error in PrepareCustomerOrderListModel() - legacyOrders.count = " + (legacyOrders != null ? Convert.ToString(legacyOrders.Count()) : "NULL"), ex, customer);

            }

            return model;
        }

        public OrderDetailsModel PrepareOrderDetailsModelLegacy(Nop.Core.Domain.Orders.Order order)
        {
            var model = new OrderDetailsModel();
            return model;
        }
    }

    public partial class OrderRouteValues : IRouteValues
    {
        public string status { get; set; }
        public int page { get; set; }
        public int? pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
