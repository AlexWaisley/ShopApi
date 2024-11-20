namespace ShopApi.Entity;

public class Product
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public List<ProductImage> Previews { get; set; }
    public required bool IsAvailable { get; set; }
    public required int? CategoryId { get; set; }
}