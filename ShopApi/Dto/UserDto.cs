using System.Text.Json.Serialization;
using ShopApi.Entity;

namespace ShopApi.Dto;

[Serializable]
public class UserDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("username")] public required string Username { get; set; }
    [JsonPropertyName("email")] public required EmailInfo Email { get; set; }
}