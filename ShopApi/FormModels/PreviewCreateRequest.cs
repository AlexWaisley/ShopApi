using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class PreviewCreateRequest
{
    [JsonPropertyName("productId")] public Guid ProductId { get; set; }
    [JsonPropertyName("fileId")] public int FileId { get; set; }
}