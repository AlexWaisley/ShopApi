using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class OrderItemDto
{
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}