using Microsoft.Data.Sqlite;
using ShopApi.Dto;
using ShopApi.Mappers;

namespace ShopApi.Data.Orders;

public class OrderRepository(string connectionString) : IOrderRepository
{
    
    public OrderDto? GetOrder(Guid id)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT OrderRecord.Id, UserId, Status, ShippingAddressId 
                             From OrderRecord  
                             where OrderRecord.Id=@Id
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new OrderDto()
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                Status = reader.GetString(2),
                ShippingAddressId = reader.GetInt32(3)
            };
        }

        return null;
    }


    public IEnumerable<OrderDto> GetUserOrders(Guid userId)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT OrderRecord.Id, UserId, Status, ShippingAddressId 
                             From OrderRecord  
                             where UserId=@UserId
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@UserId", userId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new OrderDto()
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                Status = reader.GetString(2),
                ShippingAddressId = reader.GetInt32(3)
            };
        }
    }


    public IEnumerable<OrderDto> GetAllOrders()
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT OrderRecord.Id, UserId, Status, ShippingAddressId 
                             From OrderRecord  
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new OrderDto()
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                Status = reader.GetString(2),
                ShippingAddressId = reader.GetInt32(3)
            };
        }
    }

    public int UpdateOrderStatus(Guid orderId, string status)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Update OrderRecord 
                             Set Status = @Status
                             Where Id = @OrderId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@OrderId", orderId);
        return command.ExecuteNonQuery();
    }


    public IEnumerable<OrderItemDto> GetOrderItems(Guid orderId)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT OrderId, ProductId, Quantity 
                             From OrderItem 
                             where OrderId=@OrderId
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@OrderId", orderId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new OrderItemDto()
            {
                OrderId = reader.GetGuid(0),
                ProductId = reader.GetGuid(1),
                Quantity = reader.GetInt32(2),
            };
        }
    }

    public int CreateOrder(int shippingAddressId, Guid userId, CartDto cartInfo)
    {
        var orderDto = OrderMapper.MapToDto(shippingAddressId, userId);
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Insert Into OrderRecord (Id, Status, UserId, ShippingAddressId)
                             VALUES (@OrderId,@Status,@UserId,@ShippingAddressId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@OrderId", orderDto.Id);
        command.Parameters.AddWithValue("@Status", orderDto.Status);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@ShippingAddressId", orderDto.ShippingAddressId);
        var status = command.ExecuteNonQuery();
        command.Parameters.Clear();
        if (status == 0)
            return 0;

        var command1 = connection.CreateCommand();
        const string query1 = """
                              Insert Into OrderItem (OrderId, ProductId, Quantity) 
                              values (@OrderId,@ProductId,@Quantity);
                              """;
        command1.CommandText = query1;
        foreach (var orderItemDto in cartInfo.Items)
        {
            command1.Parameters.Clear();
            command1.Parameters.AddWithValue("@OrderId", orderDto.Id);
            command1.Parameters.AddWithValue("@ProductId", orderItemDto.ProductId);
            command1.Parameters.AddWithValue("@Quantity", orderItemDto.Quantity);
            command1.ExecuteNonQuery();
        }

        var command2 = connection.CreateCommand();
        const string query2 = "Delete From CartItem where CartId = @CartId";
        command2.CommandText = query2;
        command2.Parameters.AddWithValue("@CartId", cartInfo.Id);
        command2.ExecuteNonQuery();

        return 1;
    }
    
    
    public IEnumerable<OrderDto> GetOrdersPart(int offset, int limit)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT OrderRecord.Id, UserId, Status, ShippingAddressId 
                             From OrderRecord  
                             limit @Limit offset @Offset
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Limit", limit);
        command.Parameters.AddWithValue("@Offset", offset);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new OrderDto()
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                Status = reader.GetString(2),
                ShippingAddressId = reader.GetInt32(3)
            };
        }
    }
}