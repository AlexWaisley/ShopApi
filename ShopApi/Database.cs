using Microsoft.Data.Sqlite;
using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;
using ShopApi.Mappers;

namespace ShopApi;

public class Database(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public User? Login(UserLoginRequest userLoginRequest)
    {
        var hashedPassword = HashHelper.ComputeSha256Hash(userLoginRequest.Password);
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT User.id, User.name, User.IsAdmin, EmailInfo.email, EmailInfo.isActive
                             FROM User
                             JOIN EmailInfo ON User.Id = EmailInfo.UserId
                             WHERE EmailInfo.email = @Email AND User.password = @Password
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Email", userLoginRequest.Email);
        command.Parameters.AddWithValue("@Password", hashedPassword);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                IsAdmin = reader.GetBoolean(2),
                EmailInfo = new EmailInfo()
                {
                    Email = reader.GetString(3),
                    IsActive = reader.GetBoolean(4)
                }
            };
        }

        return null;
    }


    public int Register(UserRegisterRequest userRegisterRequestData)
    {
        var user = userRegisterRequestData.MapToEntity();
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert into User (Id, Name, Password) VALUES (@UserId, @UserName, @Password);
                             INSERT INTO EmailInfo (Email, IsActive, UserId) VALUES (@Email, @IsActive, @UserId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Email", user.EmailInfo.Email);
        command.Parameters.AddWithValue("@Password", user.Password);
        command.Parameters.AddWithValue("@UserName", user.Name);
        command.Parameters.AddWithValue("@IsActive", user.EmailInfo.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("@UserId", user.Id);
        return command.ExecuteNonQuery();
    }

    public int UpdatePassword(UserPasswordUpdateRequest userPasswordUpdateRequest, Guid userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Update User 
                             Set Password = @NewPassword
                             Where Id=@UserId and Password=@CurrentPassword;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@NewPassword", userPasswordUpdateRequest.NewPassword);
        command.Parameters.AddWithValue("@CurrentPassword", userPasswordUpdateRequest.CurrentPassword);
        command.Parameters.AddWithValue("@UserId", userId);
        return command.ExecuteNonQuery();
    }

    public int UpdateInfo(UserUpdateInfoRequest userUpdateInfoRequest, Guid userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Update User 
                             Set Name = @NewName
                             Where Id = @UserId;
                             UPDATE EmailInfo
                             set Email = @NewEmail
                             where UserId = @UserId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@NewName", userUpdateInfoRequest.Name);
        command.Parameters.AddWithValue("@NewEmail", userUpdateInfoRequest.Email);
        command.Parameters.AddWithValue("@UserId", userId);
        return command.ExecuteNonQuery();
    }

    public CartDto? GetUserCart(Guid userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Cart.Id, CartItem.id, CartItem.CartId, CartItem.Quantity
                             FROM CartItem
                             JOIN Cart ON CartItem.CartId = Cart.Id
                             WHERE Cart.UserId = @UserId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@UserId", userId.ToString());
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new CartDto()
            {
                Id = reader.GetGuid(0),
                Items =
                [
                    new CartItemDto
                    {
                        Id = reader.GetGuid(1),
                        ProductId = reader.GetGuid(2),
                        Quantity = reader.GetInt32(3)
                    }
                ]
            };
        }

        return null;
    }

    public int AddToCart(CartItemDto cartItemDto)
    {
        using var connection = new SqliteConnection(_connectionString);
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


    public int RemoveFromCart(Guid cartItemId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Delete From CartItem where Id = @CartItemId";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CartItemId", cartItemId.ToString());
        return command.ExecuteNonQuery();
    }

    public IEnumerable<ProductDto> GetProducts(int count, int offset)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Id, Name, Price, IsAvailable, CategoryId 
                             From Product limit @Count offset @Offset
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Count", count);
        command.Parameters.AddWithValue("@Offset", offset);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            yield return new ProductDto()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(3),
                CategoryId = reader.GetInt32(4),
                IsAvailable = reader.GetBoolean(5)
            };
        }
    }


    public ProductDto? GetProductById(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Id, Name, Price, IsAvailable, CategoryId 
                             From Product 
                             where Id=@Id 
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new ProductDto()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(3),
                CategoryId = reader.GetInt32(4),
                IsAvailable = reader.GetBoolean(5)
            };
        }

        return null;
    }

    public IEnumerable<ProductDto> GetProductsByCategory(int categoryId, int count, int offset)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Id, Name, Price, IsAvailable, CategoryId 
                             From Product 
                             where CategoryId=@CategoryId 
                             limit @Count 
                                 offset @Offset
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Count", count);
        command.Parameters.AddWithValue("@CategoryId", categoryId);
        command.Parameters.AddWithValue("@Offset", offset);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            yield return new ProductDto()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(3),
                CategoryId = reader.GetInt32(4),
                IsAvailable = reader.GetBoolean(5)
            };
        }
    }


    public ProductFullDto? GetProductInfo(Guid productId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Id, Name, Description, Price, IsAvailable, CategoryId 
                             From Product 
                             where Id=@ProductId
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", productId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new ProductFullDto()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                CategoryId = reader.GetInt32(4),
                IsAvailable = reader.GetBoolean(5)
            };
        }

        return null;
    }

    public IEnumerable<ProductImage> GetProductPreviews(Guid productId, int count, int offset)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Url 
                             From Image 
                                 JOIN ProductImage On Image.Id = ProductImage.ImageId 
                             where ProductImage.ProductId=@ProductId
                             limit @Count offset @Offset;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@Count", count);
        command.Parameters.AddWithValue("@Offset", offset);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            yield return new ProductImage()
            {
                Url = reader.GetString(0),
                ProductId = productId
            };
        }
    }

    public IEnumerable<CategoryDto> GetAllCategories()
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Category.Id, ParentCategoryId, name, Url
                                 from Image 
                                     join Category on Image.Id = Category.ImageId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            yield return new CategoryDto()
            {
                Id = reader.GetInt32(0),
                ParentCategory = reader.GetInt32(1),
                Name = reader.GetString(2),
                ImageUrl = reader.GetString(3)
            };
        }
    }
    public IEnumerable<CategoryDto> GetCategoriesByParentCategoryId(int parentCategoryId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT Category.Id, ParentCategoryId, name, Url
                                 from Image 
                                     join Category on Image.Id = Category.ImageId
                             where ParentCategoryId=@ParentCategoryId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ParentCategoryId", parentCategoryId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            yield return new CategoryDto()
            {
                Id = reader.GetInt32(0),
                ParentCategory = reader.GetInt32(1),
                Name = reader.GetString(2),
                ImageUrl = reader.GetString(3)
            };
        }
    }

    public OrderDto? GetOrder(Guid id)
    {
        using var connection = new SqliteConnection(_connectionString);
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
        using var connection = new SqliteConnection(_connectionString);
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
        if (reader.Read())
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


    public IEnumerable<OrderItemDto> GetOrderItems(Guid orderId)
    {
        using var connection = new SqliteConnection(_connectionString);
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
        if (reader.Read())
        {
            yield return new OrderItemDto()
            {
                OrderId = reader.GetGuid(0),
                ProductId = reader.GetGuid(1),
                Quantity = reader.GetInt32(2),
            };
        }
    }

    public int CreateOrder(int shippingAddressId, Guid userId)
    {
        var orderDto = OrderMapper.MapToDto(shippingAddressId,userId);
        using var connection = new SqliteConnection(_connectionString);
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
        
        var cartInfo = GetUserCart(userId);
        if (cartInfo is null)
            return 0;
        
        const string query1 = """
                              Insert Into OrderItem (OrderId, ProductId, Quantity) 
                              values (@OrderId,@ProductId,@Quantity);
                              """;
        command.CommandText = query1;
        command.Parameters.AddWithValue("@OrderId", orderDto.Id);
        foreach (var orderItemDto in cartInfo.Items)
        {
            command.Parameters.AddWithValue("@ProductId", orderItemDto.ProductId);
            command.Parameters.AddWithValue("@Quantity", orderItemDto.Quantity);
            command.ExecuteNonQuery();
        }
        
        const string query2 = "Delete From CartItem where CartId = @CartId";
        command.CommandText = query2;
        command.Parameters.AddWithValue("@CartId", cartInfo.Id);
        command.ExecuteNonQuery();
        
        return 1;
    }

    public int AddCategory(CategoryCreateRequest categoryCreateRequest)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert INTO Image (Url)
                             values (@ImageUrl);
                             Insert Into Category (ParentCategoryId, Name, ImageId)
                             VALUES (@ParentCategoryId,@Name,last_insert_rowid());
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ParentCategoryId", categoryCreateRequest.ParentCategory);
        command.Parameters.AddWithValue("@Name", categoryCreateRequest.Name);
        command.Parameters.AddWithValue("@ImageUrl", categoryCreateRequest.ImageUrl);
        return command.ExecuteNonQuery();
    }

    public int AddProduct(Product product)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert INTO Product (Id, Name, Description, Price, IsAvailable, CategoryId)
                             values (@ProductId,@Name,@Description,@Price,@IsAvailable,@CategoryId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", product.Id);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", product.Description);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@IsAvailable", product.IsAvailable);
        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
        
        return command.ExecuteNonQuery();
    }
    
    public int AddPreviews(Guid productId, IEnumerable<string> previews)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert INTO Image (Url)
                             values (@ImageUrl);
                             Insert INTO ProductImage (ProductId, ImageId)
                             values (@ProductId, last_insert_rowid());
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", productId);
        foreach (var preview in previews)
        {
            command.Parameters.AddWithValue("@ImageUrl", preview);
            command.ExecuteNonQuery();
        }

        return 1;
    }
}