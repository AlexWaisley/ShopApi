using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;

namespace ShopApi.Controllers;

public class ProductController
{
    [HttpGet]
    public Task<IEnumerable<ProductDto>> GetProducts(int count)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{id}")]
    public Task<ProductFullDto> GetFullProduct(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("category/{categoryId}")]
    public Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("preview/{id}&{count}")]
    public Task<IEnumerable<ProductDto>> GetProductPreviews(int id, int count)
    {
        throw new NotImplementedException();
    }
    
    
    
    
    
}