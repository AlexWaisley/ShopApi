namespace ShopApi.Entity;

public class User
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required EmailInfo EmailInfo { get; set; }
    public required bool IsAdmin { get; set; }
    public string Password { get; set; }
    public List<OrderRecord> OrderHistory { get; set; } = [];
    public Cart Cart { get; set; }
}