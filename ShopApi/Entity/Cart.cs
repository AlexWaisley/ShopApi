namespace ShopApi.Entity;

public class Cart
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required List<CartItem> Items { get; set; }
}