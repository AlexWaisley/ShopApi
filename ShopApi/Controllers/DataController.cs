using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.Entity;

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
}