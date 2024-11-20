using Newtonsoft.Json;

namespace ShopApi.FormModels;

[Serializable]
public class UserUpdateInfoRequest
{
    [JsonProperty("email")]
    public required string Email { get; set; }
    [JsonProperty("name")]
    public required string Name { get; set; }
}