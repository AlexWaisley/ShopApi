using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Data.Products;

public interface IProductRepository
{
    IEnumerable<ProductDto> GetProducts(int count, int offset);
    ProductDto? GetProductById(Guid id);
    IEnumerable<ProductDto> GetProductsByCategory(int categoryId, int count, int offset);
    ProductFullDto? GetProductInfo(Guid productId);
    IEnumerable<ProductImage> GetProductPreviews(Guid productId, int count, int offset);
    int AddProduct(Product product);
    int DeleteProductPreview(int id);
    int AddProductPreview(ProductPreviewCreateRequest productPreviewCreateRequest);
    int UpdateProduct(ProductUpdateRequest productUpdateRequest);
    int DeleteProduct(Guid id);

}