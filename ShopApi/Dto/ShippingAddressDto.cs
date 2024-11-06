using System.Text.Json.Serialization;

namespace ShopApi.Dto;

[Serializable]
public class ShippingAddressDto
{
    [JsonPropertyName("id")] public required int Id { get; set; }
    
    [JsonPropertyName("city")] public required string City { get; set; }
    [JsonPropertyName("street")] public required string Street { get; set; }
    [JsonPropertyName("house")] public required string House { get; set; }
}