using Microsoft.Data.Sqlite;
using ShopApi.Data.Cart;
using ShopApi.Data.Categories;
using ShopApi.Data.Files;
using ShopApi.Data.Orders;
using ShopApi.Data.Products;
using ShopApi.Data.Users;
using ShopApi.Dto;

namespace ShopApi;

public class Database(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public IUserRepository UserRepository => new UserRepository(_connectionString);
    public IProductRepository ProductRepository => new ProductRepository(_connectionString);
    public ICategoryRepository CategoryRepository => new CategoryRepository(_connectionString);
    public IFileRepository FileRepository => new FileRepository(_connectionString);
    public IOrderRepository OrderRepository => new OrderRepository(_connectionString);
    public ICartRepository CartRepository => new CartRepository(_connectionString);
    

    public IEnumerable<ShippingAddressDto> GetAllShippingAddress()
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Id, City, Street, House
                             FROM ShippingAddress;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new ShippingAddressDto()
            {
                Id = reader.GetInt32(0),
                City = reader.GetString(1),
                Street = reader.GetString(2),
                House = reader.GetString(3)
            };
        }
    }

    public int AddRefreshToken(Guid tokenId, Guid userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert INTO RefreshToken (Id, UserId)
                             values (@TokenId,@UserId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@TokenId", tokenId);
        command.Parameters.AddWithValue("@UserId", userId);

        return command.ExecuteNonQuery();
    }

    public string? GetRefreshTokenValue(Guid token)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Select UserId from RefreshToken where Id=@TokenId";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@TokenId", token);
        using var reader = command.ExecuteReader();
        return reader.Read() ? reader.GetString(0) : null;
    }


}