using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.Entity;

namespace ShopApi.Controllers;

public class ProductController
{
    [HttpGet]
    public Task<IEnumerable<ProductDto>> GetProducts(int count, int offset)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("{id}")]
    public Task<ProductFullDto> GetFullProduct(int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("category/{categoryId}")]
    public Task<IEnumerable<ProductDto>> GetProductsByCategory(int categoryId, int count , int offset)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("preview/{id}&{count}")]
    public Task<IEnumerable<ProductImage>> GetProductPreviews(int id, int count)
    {
        throw new NotImplementedException();
    }
    
    
    
    
    
}