using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class CartDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    [JsonPropertyName("items")]
    public List<CartItemDto> Items { get; set; }
}