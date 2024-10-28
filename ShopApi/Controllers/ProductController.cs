using Microsoft.AspNetCore.Mvc;

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
}