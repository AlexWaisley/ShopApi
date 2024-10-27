using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class CartItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    [JsonPropertyName("cartId")]
    public Guid CartId { get; set; }
}