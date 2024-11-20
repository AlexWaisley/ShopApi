namespace ShopApi.Entity;

public class CartItem
{
    public required Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public Product Product { get; set; }
    public required int Quantity { get; set; }
    public decimal Price => Product.Price*Quantity;

    public required Guid CartId { get; set; }
}