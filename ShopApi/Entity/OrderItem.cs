namespace ShopApi.Entity;

public class OrderItem
{
    public required Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public required Product Product { get; set; }
    public decimal Price => Product.Price*Quantity;
    public required int Quantity { get; set; }
    public required Guid OrderRecordId { get; set; }
}