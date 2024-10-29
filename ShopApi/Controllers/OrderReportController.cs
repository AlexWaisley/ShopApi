using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.FormModels;

namespace ShopApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderReportController(Database database) : ControllerBase
{
    private readonly ILogger<OrderReportController> _logger;


    [HttpGet("/orders/all/info")]
    public IActionResult GetOrder([FromBody] Guid orderId)
    {
        var result = database.GetOrder(orderId);

        if (result is not null)
            return Ok(result);
        return NotFound();
    }


    [HttpGet("/orders/user")]
    public IActionResult GetUserOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.GetUserOrders(id);
        if (result.Any())
            return Ok(result);

        return NotFound();
    }

    [HttpGet("/orders/all/info/items")]
    public IActionResult GetOrderItems([FromBody] Guid orderId)
    {
        var result = database.GetOrderItems(orderId);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpPost("/orders/create")]
    public IActionResult CreateOrder(OrderCreateRequest orderCreateRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.CreateOrder(orderCreateRequest,id);
        if (result > 0)
            return Ok(result);

        return NotFound();
    }
}