using Microsoft.Data.Sqlite;
using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Data.Products;

public class ProductRepository(string connectionString) : IProductRepository
{
    
    public IEnumerable<ProductDto> GetProducts(int count, int offset)
    {
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
                IsAvailable = reader.GetBoolean(4),
                CategoryId = reader.GetInt32(5),
            };
        }

        return null;
    }

    public IEnumerable<ProductImage> GetProductPreviews(Guid productId, int count, int offset)
    {
        using var connection = new SqliteConnection(connectionString);
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
    
    
    public int AddProduct(Product product)
    {
        using var connection = new SqliteConnection(connectionString);
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
    
    public int DeleteProduct(Guid id)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Delete From Product where Id = @Id";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
        return command.ExecuteNonQuery();
    }

    public int DeleteProductPreview(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Delete From ProductImage where ImageId = @Id";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
        return command.ExecuteNonQuery();
    }
    
    
    public int AddProductPreview(ProductPreviewCreateRequest productPreviewCreateRequest)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Insert INTO ProductImage (ProductId, ImageId)
                             values (@ProductId, @ImageId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ProductId", productPreviewCreateRequest.ProductId);
        command.Parameters.AddWithValue("@ImageId", productPreviewCreateRequest.FileId);
        return command.ExecuteNonQuery();
    }

    
    public int UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Update Product 
                             Set Name = @Name, Description = @Description, Price = @Price, IsAvailable = @IsAvailable, CategoryId = @CategoryId
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
        command.Parameters.AddWithValue("@CategoryId", productUpdateRequest.CategoryId);
        return command.ExecuteNonQuery();
    }

}