using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class CategoryPreviewUpdateRequest
{
    [JsonPropertyName("categoryId")] public int CategoryId { get; set; }
    [JsonPropertyName("fileId")] public int FileId { get; set; }
}