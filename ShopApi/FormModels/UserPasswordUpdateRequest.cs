using Newtonsoft.Json;

namespace ShopApi.FormModels;

[Serializable]
public class UserPasswordUpdateRequest
{
    [JsonProperty("oldPassword")]
    public string CurrentPassword { get; set; }
    [JsonProperty("newPassword")]
    public string NewPassword { get; set; }
}