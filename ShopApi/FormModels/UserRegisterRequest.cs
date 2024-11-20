using Newtonsoft.Json;
using ShopApi.Entity;

namespace ShopApi.FormModels;

[Serializable]
public class UserRegisterRequest
{
    [JsonProperty("name")]
    public required string Name { get; set; }
    [JsonProperty("emailInfo")]
    public required string Email { get; set; }
    [JsonProperty("password")]
    public required string Password { get; set; }
}