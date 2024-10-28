using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderReportController(Database database) : ControllerBase
{
    private readonly ILogger<OrderReportController> _logger;


    [HttpGet("/orders/all/info")]
    public IActionResult GetOrder([FromBody]Guid orderId)
    {
        var result = database.GetOrder(orderId);
        
        if (result is not null)
            return Ok(result);
        return NotFound();
    }
    
    
    [HttpGet("/orders/user")]
    public async Task<IEnumerable<OrderDto>> GetUserOrders()
    {
        throw new NotImplementedException();

    }
    
    [HttpGet("/orders/all/info/items")]
    public IActionResult GetOrderItems([FromBody]Guid orderId)
    {
        var result = database.GetOrderItems(orderId);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }
    
    [HttpPost("/orders/create")]
    public async Task<OrderDto> CreateOrder(OrderDto order)
    {
        throw new NotImplementedException();
    }
}