namespace ShopApi.Entity;

public class ProductImage
{
    public required string Url { get; set; }
    public required Guid ProductId { get; set; }
}