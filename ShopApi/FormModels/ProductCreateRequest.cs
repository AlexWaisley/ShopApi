using System.Text.Json.Serialization;
using ShopApi.Entity;

namespace ShopApi.FormModels;

[Serializable]
public class ProductCreateRequest
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("categoryId")] public required int CategoryId { get; set; }
    [JsonPropertyName("price")] public required decimal Price { get; set; }
    [JsonPropertyName("description")] public required string Description { get; set; }
}