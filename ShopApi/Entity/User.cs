namespace ShopApi.Entity;

public class User
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required EmailInfo EmailInfo { get; set; }
    public required string Password { get; set; }
    public required List<Order> OrderHistory { get; set; }
    public required List<Cart> Cart { get; set; }
}