using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class CartItemAddRequest
{
    [JsonPropertyName("productId")] public required Guid ProductId { get; set; }
    [JsonPropertyName("quantity")] public required int Quantity { get; set; }
}