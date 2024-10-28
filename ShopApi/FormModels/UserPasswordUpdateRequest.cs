using Newtonsoft.Json;

namespace ShopApi.FormModels;

[Serializable]
public class UserPasswordUpdateRequest
{
    [JsonProperty("oldPassword")]
    public required string CurrentPassword { get; set; }
    [JsonProperty("newPassword")]
    public required string NewPassword { get; set; }
}