using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Catalog;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Order.GBS;
using Nop.Services.Payments;
using Nop.Plugin.Order.GBS.Orders;
using Newtonsoft.Json;

namespace Nop.Services.Custom.Orders
{
    public class GBSOrderService : Nop.Services.Orders.OrderService
    {
        #region Fields

        private readonly IRepository<Nop.Core.Domain.Orders.Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;

        #endregion
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="orderItemRepository">Order item repository</param>
        /// <param name="orderNoteRepository">Order note repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="eventPublisher">Event published</param>
        public GBSOrderService(IRepository<Nop.Core.Domain.Orders.Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<Customer> customerRepository,
            IEventPublisher eventPublisher,
            IPluginFinder pluginFinder,
            IStoreContext storeContext) : base(orderRepository, orderItemRepository, orderNoteRepository, productRepository, recurringPaymentRepository, customerRepository, eventPublisher)
        {
            this._orderRepository = orderRepository;
            this._orderItemRepository = orderItemRepository;
            this._orderNoteRepository = orderNoteRepository;
            this._productRepository = productRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
        }


        #endregion
        public Nop.Core.Domain.Orders.OrderItem GetOrderItemById(int orderItemId, bool useBase)
        {
            if (useBase)
            {
                return base.GetOrderItemById(orderItemId);
            }
            return GetOrderItemById(orderItemId);
        }

        public Nop.Core.Domain.Orders.Order GetOrderById(int orderId, bool useBase = false)
        {
            if (useBase)
            {
                return base.GetOrderById(orderId);
            }
            return GetOrderById(orderId);
        }
        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        public override Nop.Core.Domain.Orders.Order GetOrderById(int orderId)
        {
            var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                var order = base.GetOrderById(orderId);
                if(order == null)
                {
                    order = new OrderExtensions().GetOrderById(orderId, true);
                }
                return order;
            }
            else {
                return base.GetOrderById(orderId);
            }

        }
        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderId">The order item identifier</param>
        /// <returns>Order</returns>
        public override Nop.Core.Domain.Orders.OrderItem GetOrderItemById(int orderItemId)
        {
            var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                var orderItem = base.GetOrderItemById(orderItemId);
                if (orderItem == null)
                {
                    orderItem = new OrderExtensions().GetOrderItemById(orderItemId, true);
                }
                return orderItem;
            }
            else
            {
                return base.GetOrderItemById(orderItemId);
            }

        }

        //public override IPagedList<Nop.Core.Domain.Orders.Order> SearchOrders(int storeId = 0,
        //    int vendorId = 0, int customerId = 0,
        //    int productId = 0, int affiliateId = 0, int warehouseId = 0,
        //    int billingCountryId = 0, string paymentMethodSystemName = null,
        //    DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        //    List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
        //    string billingEmail = null, string billingLastName = "",
        //    string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue)
        //{
        //    IPagedList<Nop.Core.Domain.Orders.Order> orders = base.SearchOrders(storeId, vendorId, customerId, productId, affiliateId, warehouseId, billingCountryId, paymentMethodSystemName, createdFromUtc, createdToUtc, osIds, psIds, ssIds,
        //        billingEmail, billingLastName, orderNotes, pageIndex, pageSize);
        //    var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id).ToList();
        //    if (miscPlugins.Count > 0)
        //    {

        //        Nop.Core.Domain.Orders.Order myOrder = new Nop.Core.Domain.Orders.Order();
        //        myOrder.Id = 99999999;
        //        Nop.Core.Domain.Orders.OrderItem myItem = new Nop.Core.Domain.Orders.OrderItem();
        //        myItem.Id = 99999999;
        //        var _productService = EngineContext.Current.Resolve<IProductService>(); ;
        //        OrderExtensions.setCustomValue("isLegacy", true, myOrder);
        //        // bool isLegacy = Convert.ToBoolean(OrderExtensions.getCustomValue("isLegacy", myOrder));
        //        myItem.OrderId = 99999999;
        //        myItem.Product = _productService.GetProductBySku("NCALH6-00001");
        //        myOrder.OrderItems.Add(myItem);
        //        orders.Add(myOrder);
        //        //orders.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        //        return orders;
        //    }else
        //    {
        //        return orders;
        //    }
        //}
    }
}
