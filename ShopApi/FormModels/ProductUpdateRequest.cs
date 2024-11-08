using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class ProductUpdateRequest
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("price")] public decimal Price { get; set; }
    [JsonPropertyName("isAvailable")] public bool IsAvailable { get; set; }
}