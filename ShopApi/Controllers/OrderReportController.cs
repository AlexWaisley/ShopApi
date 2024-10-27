using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;

namespace ShopApi.Controllers;

public class OrderReportController
{
    private readonly ILogger<OrderReportController> _logger;

    public OrderReportController(ILogger<OrderReportController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "order")]
    public async Task<IEnumerable<OrderDto>> GetOrder(string orderId)
    {
        throw new NotImplementedException();
    }
    [HttpGet(Name = "user-orders")]
    //user id from cookie
    public async Task<IEnumerable<OrderDto>> GetUserOrders()
    {
        throw new NotImplementedException();
    }
    [HttpGet(Name="order-items")]
    public async Task<IEnumerable<OrderItemDto>> GetOrderItems(string orderId)
    {
        throw new NotImplementedException();
    }
    [HttpPost(Name="new-order")]
    public async Task<OrderDto> CreateOrder(OrderDto order)
    {
        throw new NotImplementedException();
    }
}