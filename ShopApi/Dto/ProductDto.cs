using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class ProductDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("isAvailable")] public required bool IsAvailable { get; set; }
    [JsonPropertyName("categoryId")] public required int CategoryId { get; set; }
    [JsonPropertyName("price")] public required decimal Price { get; set; }
}