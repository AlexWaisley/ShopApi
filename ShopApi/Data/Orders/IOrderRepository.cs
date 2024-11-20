using ShopApi.Dto;

namespace ShopApi.Data.Orders;

public interface IOrderRepository
{
    OrderDto? GetOrder(Guid id);
    IEnumerable<OrderDto> GetUserOrders(Guid userId);
    IEnumerable<OrderItemDto> GetOrderItems(Guid orderId);
    IEnumerable<OrderDto> GetAllOrders();
    int UpdateOrderStatus(Guid orderId, string status);
    int CreateOrder(int shippingAddressId, Guid userId, CartDto cartInfo);
    IEnumerable<OrderDto> GetOrdersPart(int offset, int limit);
    
}