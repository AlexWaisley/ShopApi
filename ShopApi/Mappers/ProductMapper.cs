using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Mappers;

public static class ProductMapper
{
    public static Product MapToEntity(this ProductCreateRequest request)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsAvailable = request.IsAvailable,
            CategoryId = request.CategoryId
        };
    }
}