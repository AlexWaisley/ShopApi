using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.FormModels;
using ShopApi.Identity;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderReportController(Database database) : ControllerBase
{
    [Authorize]
    [HttpGet("/orders/{orderId:guid}/all/info")]
    public IActionResult GetOrder(Guid orderId)
    {
        var order = database.OrderRepository.GetOrder(orderId);
        if (order is null)
            return NotFound();
        var orderItems = database.OrderRepository.GetOrderItems(orderId).ToList();
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

    [Authorize]
    [HttpGet("/orders/user")]
    public IActionResult GetUserOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.OrderRepository.GetUserOrders(id).ToList();

        if (result.Count != 0)
            return Ok(result);

        return NotFound();
    }

    [Authorize]
    [HttpGet("/orders/all/{orderId:guid}/info/items")]
    public IActionResult GetOrderItems(Guid orderId)
    {
        var result = database.OrderRepository.GetOrderItems(orderId);
        if (result.Any())
            return Ok(result);
        return NotFound();
    }
    
    [Authorize (Policy = IdentityData.AdminUserPolicyName)]
    [HttpGet("/orders/all")]
    public IActionResult GetAllOrders()
    {
        var result = database.OrderRepository.GetAllOrders().ToList();
        if (result.Count != 0)
            return Ok(result);
        return NotFound();
    }
    
    [Authorize (Policy = IdentityData.AdminUserPolicyName)]
    [HttpGet("/orders/all/offset={offset:int}&limit={limit:int}")]
    public IActionResult GetOrdersPart(int offset, int limit)
    {
        var result = database.OrderRepository.GetOrdersPart(offset, limit).ToList();
        if (result.Count != 0)
            return Ok(result);
        return NotFound();
    }
    
    [Authorize (Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost("/orders/update")]
    public IActionResult UpdateOrderStatus([FromBody] OrderStatusUpdateRequest request)
    {
        var result = database.OrderRepository.UpdateOrderStatus(request.OrderId, request.Status);
        if (result > 0)
            return Ok();
        return NotFound();
    }

    [Authorize]
    [HttpPost("/orders/create/{shippingAddressId:int}")]
    public IActionResult CreateOrder(int shippingAddressId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var cartInfo = database.CartRepository.GetUserCart(id);
        if (cartInfo == null) return NotFound();
        var result = database.OrderRepository.CreateOrder(shippingAddressId, id, cartInfo);
        if (result > 0)
            return Ok(result);

        return NotFound();
    }
}