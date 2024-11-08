using System.Globalization;
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
        var result = command.ExecuteNonQuery();
        
        return result == 0 ? 0 : CreateCartForUser(user.Id);
    }

    private int CreateCartForUser(Guid userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert into Cart (UserId)
                             values (@UserId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@UserId", userId);
        return command.ExecuteNonQuery();
    }

    public User? GetUserById(string userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             SELECT User.id, User.name, User.IsAdmin, EmailInfo.email, EmailInfo.isActive
                             FROM User
                             JOIN EmailInfo ON User.Id = EmailInfo.UserId
                             WHERE User.Id = @UserId
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@UserId", userId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                IsAdmin = reader.GetBoolean(2),
                EmailInfo = new EmailInfo
                {
                    Email = reader.GetString(3),
                    IsActive = reader.GetBoolean(4)
                }
            };
        }

        return null;
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
    
    public int UpdateQuantity(CartItemDto cartItemDto)
    {
        using var connection = new SqliteConnection(_connectionString);
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
        var cartId = GetCartId(userId);
        using var connection = new SqliteConnection(_connectionString);
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
        if(cartId == -1)
            return null;
        return new CartDto()
        {
            Id = cartId,
            Items = cartItems
        };
    }

    private int GetCartId(Guid userId)
    {
        using var connection = new SqliteConnection(_connectionString);
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


    public int RemoveFromCart(int cartItemId)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Delete From CartItem where Id = @CartItemId";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CartItemId", cartItemId);
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
                Price = reader.GetDecimal(2),
                CategoryId = reader.GetInt32(4),
                IsAvailable = reader.GetBoolean(3)
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
        while (reader.Read())
        {
            yield return new ProductDto()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                CategoryId = reader.GetInt32(4),
                IsAvailable = reader.GetBoolean(3)
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
                             SELECT ImageId 
                             From ProductImage
                             where ProductId=@ProductId
                             limit @Count offset @Offset;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@Count", count);
        command.Parameters.AddWithValue("@Offset", offset);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new ProductImage()
            {
                ImageId = reader.GetInt32(0),
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
        while (reader.Read())
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
                             where Category.ParentCategoryId=@ParentCategoryId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ParentCategoryId", parentCategoryId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
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
        using var connection = new SqliteConnection(_connectionString);
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
        using var connection = new SqliteConnection(_connectionString);
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

    private int AddImage(string url)
    {
        //return image id
        var imageId = IsImageExists(url);
        if (imageId != -1)
            return imageId;
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Insert INTO Image (Url) values (@ImageUrl);";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ImageUrl", url);
        command.ExecuteNonQuery();
        command.CommandText = "SELECT last_insert_rowid();";
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetInt32(0);
        }

        return -1;
    }

    private int IsImageExists(string imageUrl)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Select Id from Image where Url=@ImageUrl";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ImageUrl", imageUrl);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetInt32(0);
        }

        return -1;
    }

    public int AddCategory(CategoryCreateRequest categoryCreateRequest)
    {
        var imageId = AddImage(categoryCreateRequest.ImageUrl);
        if (imageId == -1)
            return 0;
        // parent category id is 0 for root categories
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert Into Category (ParentCategoryId, Name, ImageId)
                             VALUES (@ParentCategoryId,@Name,@ImageId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ParentCategoryId", categoryCreateRequest.ParentCategoryId);
        command.Parameters.AddWithValue("@Name", categoryCreateRequest.Name);
        command.Parameters.AddWithValue("@ImageId", imageId);
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

    public IEnumerable<DbFile> GetFiles()
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Select Id, Size, Sha256, Name, CreatedAt, ContentType from Files";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            yield return new DbFile()
            {
                Id = reader.GetInt32(0),
                Size = reader.GetInt32(1),
                Sha256 = reader.GetString(2),
                Name = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4),
                ContentType = reader.GetString(5)
            };
        }
    }
    
    
    public DbFile? GetFileById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = "Select Id, Size, Sha256, Name, CreatedAt, ContentType from Files where Id = @Id";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if(reader.Read())
        {
            return new DbFile()
            {
                Id = reader.GetInt32(0),
                Size = reader.GetInt32(1),
                Sha256 = reader.GetString(2),
                Name = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4),
                ContentType = reader.GetString(5)
            };
        }

        return null;
    }

    public int AddFile(long size, string name, string sha256, string contentType)
    {
        var time = DateTime.UtcNow;
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             INSERT INTO Files (Size, Sha256, Name, CreatedAt, ContentType)
                             values (@Size,@Sha256,@Name,@CreatedAt, @ContentType) RETURNING Id;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Size", size);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Sha256", sha256);
        command.Parameters.AddWithValue("@ContentType", contentType);
        command.Parameters.AddWithValue("@CreatedAt", time.ToString(CultureInfo.InvariantCulture));

        var reader = command.ExecuteReader(); 
        
        if (reader.Read())
        {
            return reader.GetInt32(0);
        }

        throw new NullReferenceException("Id is null");
    }

    public int? GetFileByHash(string hash)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                                Select Id from Files where Sha256 = @Hash;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Hash", hash);

        var reader = command.ExecuteReader(); 
        
        if (reader.Read())
        {
            return reader.GetInt32(0);
        }

        return null;
    }
    
    public IEnumerable<OrderDto> GetOrdersPart(int offset, int limit)
    {
        using var connection = new SqliteConnection(_connectionString);
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


    public int AddPreview(PreviewCreateRequest previewCreateRequest)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Insert INTO ProductImage (ProductId, ImageId)
                             values (@ProductId, @ImageId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", previewCreateRequest.ProductId);
        command.Parameters.AddWithValue("@ImageId", previewCreateRequest.FileId);
        return command.ExecuteNonQuery();
    }

    public int UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string query = """
                             Update Product 
                             Set Name = @Name, Description = @Description, Price = @Price, IsAvailable = @IsAvailable
                             Where Id = @ProductId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", productUpdateRequest.Id);
        command.Parameters.AddWithValue("@Name", productUpdateRequest.Name);
        command.Parameters.AddWithValue("@Description", productUpdateRequest.Description);
        command.Parameters.AddWithValue("@Price", productUpdateRequest.Price);
        command.Parameters.AddWithValue("@IsAvailable", productUpdateRequest.IsAvailable);
        return command.ExecuteNonQuery();
    }
}