using System.Text.Json.Serialization;
using ShopApi.Dto;

namespace ShopApi.FormModels;

[Serializable]
public class OrderCreateRequest
{
    [JsonPropertyName("shippingAddress")] public required int ShippingAddressId { get; set; }
    [JsonPropertyName("orderItemsRequest")] public required List<OrderItemRequest> OrderItemsRequest { get; set; }
}