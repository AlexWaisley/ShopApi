using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.FormModels;
using ShopApi.Identity;
using ShopApi.Mappers;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(Database database) : ControllerBase
{
    [HttpGet("/products/general/count={count:int}&offset={offset:int}")]
    public IActionResult GetProducts(int count, int offset)
    {
        var result = database.GetProducts(count, offset);

        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/products/full")]
    public IActionResult GetFullProduct([FromBody] Guid id)
    {
        var result = database.GetProductInfo(id);
        if (result is not null)
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/products/category/count={count:int}&offset={offset:int}")]
    public IActionResult GetProductsByCategory([FromBody] int categoryId, int count, int offset)
    {
        var result = database.GetProductsByCategory(categoryId, count, offset);

        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/products/previews/count={count:int}&offset={offset:int}")]
    public IActionResult GetProductPreviews([FromBody] Guid id, int count, int offset)
    {
        var result = database.GetProductPreviews(id, count, offset);
        
        if (result.Any())
            return Ok(result);
        return NotFound();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/products/add")]
    public IActionResult AddProduct([FromBody] ProductCreateRequest productCreateRequest)
    {
        var product = productCreateRequest.MapToEntity();
        var result = database.AddProduct(product);
        var result1 = database.AddPreviews(product.Id, productCreateRequest.Previews);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
}