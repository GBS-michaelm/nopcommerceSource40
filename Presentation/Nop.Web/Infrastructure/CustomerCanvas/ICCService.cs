using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace Nop.Web.Infrastructure
{
    public interface ICcService
    {
        bool IsPluginEnabled();

        bool IsProductForCc(Product product);

        bool IsProductForCc(int productId);

        OrderItem GetOrderItemById(int orderItemId, bool useBase);

        OrderItem GetOrderItemById(int orderItemId);

        OrderItem GetOrderItemById_Legacy(int orderItemId, bool isLegacy);

        OrderItem prepareLegacyOrderItem(OrderItem orderItem);
    }
}
