using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class CartItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    [JsonPropertyName("cartId")]
    public int CartId { get; set; }
}