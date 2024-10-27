using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class OrderDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("userId")] public required Guid UserId { get; set; }
    [JsonPropertyName("status")] public required string Status { get; set; }
    [JsonPropertyName("shippingAddress")] public required string ShippingAddress { get; set; }
}