using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class OrderReportFull
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("userId")] public required Guid UserId { get; set; }
    [JsonPropertyName("status")] public required string Status { get; set; }
    [JsonPropertyName("shippingAddress")] public required int ShippingAddressId { get; set; }
    [JsonPropertyName("items")] public required List<OrderItemDto> Items { get; set; }
}