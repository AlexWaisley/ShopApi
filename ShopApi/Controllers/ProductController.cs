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
        var result = database.ProductRepository.GetProducts(count, offset);
        return Ok(result);
    }

    [HttpGet("/products/general/id={id:guid}")]
    public IActionResult GetProductById(Guid id)
    {
        var result = database.ProductRepository.GetProductById(id);
        if (result is not null)
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/products/id={id:guid}/full")]
    public IActionResult GetFullProduct(Guid id)
    {
        var result = database.ProductRepository.GetProductInfo(id);
        if (result is not null)
            return Ok(result);
        return NotFound();
    }

    [HttpGet("/products/category/id={categoryId:int}/count={count:int}&offset={offset:int}")]
    public IActionResult GetProductsByCategory(int categoryId, int count, int offset)
    {
        var result = database.ProductRepository.GetProductsByCategory(categoryId, count, offset).ToList();
        return Ok(result);
    }

    [HttpGet("/products/id={id:guid}/previews/count={count:int}&offset={offset:int}")]
    public IActionResult GetProductPreviews(Guid id, int count, int offset)
    {
        var result = database.ProductRepository.GetProductPreviews(id, count, offset);
        
        if (result.Any())
            return Ok(result);
        return NotFound();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/products/add/preview")]
    public IActionResult AddPreview([FromBody] ProductPreviewCreateRequest productPreviewCreateRequest)
    {
        var result = database.ProductRepository.AddProductPreview(productPreviewCreateRequest);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/products/add")]
    public IActionResult AddProduct([FromBody] ProductCreateRequest productCreateRequest)
    {
        var product = productCreateRequest.MapToEntity();
        var result = database.ProductRepository.AddProduct(product);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/products/update")]
    public IActionResult UpdateProduct([FromBody] ProductUpdateRequest productUpdateRequest)
    {
        var result = database.ProductRepository.UpdateProduct(productUpdateRequest);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpDelete("/products/id={id:guid}")]
    public IActionResult DeleteProduct(Guid id)
    {
        var result = database.ProductRepository.DeleteProduct(id);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpDelete("/products/preview/id={id:int}")]
    public IActionResult DeleteProductPreview(int id)
    {
        var result = database.ProductRepository.DeleteProductPreview(id);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
}