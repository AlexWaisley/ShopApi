using ShopApi.Dto;

namespace ShopApi.Data.Cart;

public interface ICartRepository
{
    int UpdateQuantity(CartItemDto cartItemDto);
    CartDto? GetUserCart(Guid userId);
    int AddToCart(CartItemDto cartItemDto);
    int RemoveFromCart(int cartItemId);
    
}