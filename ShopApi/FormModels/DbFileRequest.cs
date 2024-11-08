using System.Text.Json.Serialization;

namespace ShopApi.FormModels;

[Serializable]
public class DbFileRequest
{
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }  
}