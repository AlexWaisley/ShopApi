using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;
using ShopApi.Identity;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController(Database database) : ControllerBase
{
    private readonly ILogger<DataController> _logger;


    [HttpGet("/categories")]
    public IActionResult GetCategories()
    {
        var result = database.GetAllCategories();
        if (result.Any())
            return Ok(result);
        return NotFound();
    }
    
    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/categories")]
    public IActionResult CreateCategory([FromBody]CategoryCreateRequest category)
    {
        var result = database.AddCategory(category);
        if (result > 0)
            return Ok();
        return BadRequest();
    }
}