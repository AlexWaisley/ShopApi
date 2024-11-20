using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class CategoryPreviewCreateRequest
{
    [JsonPropertyName("categoryId")] public int CategoryId { get; set; }
    [JsonPropertyName("fileId")] public int FileId { get; set; }
}