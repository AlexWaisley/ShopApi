using Newtonsoft.Json;

namespace ShopApi.FormModels;

[Serializable]
public class UserLoginRequest
{
    [JsonProperty("email")]
    public required string Email { get; set; }
    [JsonProperty("password")]
    public required string Password { get; set; }
    
}