using Nop.Services.Orders;

namespace Nop.Plugin.Order.GBS.Orders
{
    public interface IGBSOrderService : IOrderService
    {
        Core.Domain.Orders.OrderItem GetOrderItemById(int orderItemId, bool useBase);

        Core.Domain.Orders.Order GetOrderById(int orderId, bool useBase = false);
    }
}
