using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class CategoryCreateRequest
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("parentCategoryId")] public required int? ParentCategoryId { get; set; }
}