using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class CategoryUpdateRequest
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("parentId")] public int ParentId { get; set; }
}