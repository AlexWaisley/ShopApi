using Microsoft.Data.Sqlite;
using ShopApi.Dto;

namespace ShopApi.Data.Cart;

public class CartRepository(string connectionString) : ICartRepository
{
    public int UpdateQuantity(CartItemDto cartItemDto)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Update CartItem 
                             Set Quantity = @Quantity
                             Where Id = @Id;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Quantity", cartItemDto.Quantity);
        command.Parameters.AddWithValue("@Id", cartItemDto.Id);
        return command.ExecuteNonQuery();
    }


    public CartDto? GetUserCart(Guid userId)
    {
        var cartId = GetCartId(userId);
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT Id, ProductId, Quantity
                             FROM CartItem where CartId = @CartId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CartId", cartId);
        using var reader = command.ExecuteReader();
        var cartItems = new List<CartItemDto>([]);
        while (reader.Read())
        {
            cartItems.Add(new CartItemDto()
            {
                Id = reader.GetInt32(0),
                ProductId = reader.GetGuid(1),
                Quantity = reader.GetInt32(2),
                CartId = cartId
            });
        }

        if (cartId == -1)
            return null;
        return new CartDto()
        {
            Id = cartId,
            Items = cartItems
        };
    }

    private int GetCartId(Guid userId)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Select Id from Cart where UserId = @UserId";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@UserId", userId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetInt32(0);
        }

        return -1;
    }

    public int AddToCart(CartItemDto cartItemDto)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Insert Into CartItem (CartId, ProductId, Quantity)
                             VALUES (@CartId,@ProductId,@Quantity);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CartId", cartItemDto.CartId);
        command.Parameters.AddWithValue("@ProductId", cartItemDto.ProductId);
        command.Parameters.AddWithValue("@Quantity", cartItemDto.Quantity);
        return command.ExecuteNonQuery();
    }

    
    

    public int RemoveFromCart(int cartItemId)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Delete From CartItem where Id = @CartItemId";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CartItemId", cartItemId);
        return command.ExecuteNonQuery();
    }

}