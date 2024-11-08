namespace ShopApi.Entity;

public class ProductImage
{
    public required int ImageId { get; set; }
    public required Guid ProductId { get; set; }
}