using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class OrderStatusUpdateRequest
{
    [JsonPropertyName("orderId")] public Guid OrderId { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
}