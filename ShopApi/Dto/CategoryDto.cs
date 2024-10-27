using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class CategoryDto
{
    [JsonPropertyName("id")] public required int Id { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("parentCategory")] public required int? ParentCategory { get; set; }
    [JsonPropertyName("imageUrl")] public required string ImageUrl { get; set; }
}