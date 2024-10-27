using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Entity;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;


    [HttpGet(Name = "categories")]
    public async Task<IEnumerable<Category>> GetCategories()
    {
        throw new NotImplementedException();
    }
}