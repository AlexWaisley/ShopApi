namespace ShopApi.Entity;

public class Order
{
    public required Guid Id { get; set; }
    public required List<OrderItem> Items { get; set; }
    public required Guid UserId { get; set; }
    public required string Status { get; set; }
    public required decimal Total { get; set; }
    public required string ShippingAddress { get; set; }
}