using System.Text.Json.Serialization;
using ShopApi.Entity;

namespace ShopApi.Dto;

[Serializable]
public class UserDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("isAdmin")] public required bool IsAdmin { get; set; }
    [JsonPropertyName("email")] public required EmailInfo Email { get; set; }
}