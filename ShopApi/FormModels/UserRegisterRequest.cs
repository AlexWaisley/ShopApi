using ShopApi.Entity;

namespace ShopApi.FormModels;

public class UserRegisterRequest
{
    public required string Name { get; set; }
    public required EmailInfo EmailInfo { get; set; }
    public string Password { get; set; }
}