using Newtonsoft.Json;
using ShopApi.Dto;

namespace ShopApi.FormModels;

[Serializable]
public class OrderCreateRequest
{
    [JsonProperty("order")] public required OrderDto OrderDto { get; set; }
    [JsonProperty("orderItems")] public required List<OrderItemDto> OrderItemsDto { get; set; }
}