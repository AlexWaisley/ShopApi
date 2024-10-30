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

    [HttpGet("/orders/{orderId:guid}/all/info")]
    public IActionResult GetOrder(Guid orderId)
    {
        var order = database.GetOrder(orderId);
        if (order is null)
            return NotFound();
        var orderItems = database.GetOrderItems(orderId).ToList();
        var result = new OrderReportFull
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            ShippingAddressId = order.ShippingAddressId,
            Items = orderItems
        };

        return Ok(result);
    }


    [HttpGet("/orders/user")]
    public IActionResult GetUserOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.GetUserOrders(id).ToList();

        if (result.Count != 0)
            return Ok(result);

        return NotFound();
    }

    [HttpGet("/orders/all/{orderId:guid}/info/items")]
    public IActionResult GetOrderItems(Guid orderId)
    {
        var result = database.GetOrderItems(orderId);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }

    [HttpPost("/orders/create")]
    public IActionResult CreateOrder([FromBody]int shippingAddressId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.CreateOrder(shippingAddressId, id);
        if (result > 0)
            return Ok(result);

        return NotFound();
    }
}