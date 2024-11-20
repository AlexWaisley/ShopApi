using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Mappers;

public static class OrderMapper
{
    public static OrderDto MapToDto(int shippingAddressId, Guid userId)
    {
        return new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShippingAddressId = shippingAddressId,
            Status = "Pending"
        };
    }
}