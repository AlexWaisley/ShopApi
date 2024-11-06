using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class CategoryCreateRequest
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("parentCategoryId")] public required int? ParentCategoryId { get; set; }
    [JsonPropertyName("imageUrl")] public required string ImageUrl { get; set; }
}