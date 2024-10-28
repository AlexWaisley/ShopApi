namespace ShopApi.Entity;

public class OrderRecord
{
    public required Guid Id { get; set; }
    public required List<OrderItem> Items { get; set; }
    public required Guid UserId { get; set; }
    public required string Status { get; set; }

    public decimal Total
    {
        get
        {
            return Items.Sum(x => x.Price * x.Quantity);
        }
    }

    public required string ShippingAddress { get; set; }
}