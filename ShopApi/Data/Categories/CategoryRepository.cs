using Microsoft.Data.Sqlite;
using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Data.Categories;

public class CategoryRepository(string connectionString) : ICategoryRepository
{
    public IEnumerable<CategoryDto> GetAllCategories()
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT Id, ParentCategoryId, name
                                 from Category;
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
                Name = reader.GetString(2)
            };
        }
    }

    public IEnumerable<CategoryDto> GetCategoriesByParentCategoryId(int parentCategoryId)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT Id, ParentCategoryId, name
                                 from  Category 
                             where ParentCategoryId=@ParentCategoryId;
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
                Name = reader.GetString(2)
            };
        }
    }

    
    public int AddCategory(CategoryCreateRequest categoryCreateRequest)
    {
        // parent category id is 0 for root categories
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Insert Into Category (ParentCategoryId, Name)
                             VALUES (@ParentCategoryId,@Name);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ParentCategoryId", categoryCreateRequest.ParentCategoryId);
        command.Parameters.AddWithValue("@Name", categoryCreateRequest.Name);
        return command.ExecuteNonQuery();
    }
    
    
    public int AddCategoryPreview(CategoryPreviewCreateRequest categoryPreviewCreateRequest)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Insert INTO CategoryImage (CategoryId, ImageId)
                             values (@CategoryId, @ImageId);
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CategoryId", categoryPreviewCreateRequest.CategoryId);
        command.Parameters.AddWithValue("@ImageId", categoryPreviewCreateRequest.FileId);
        return command.ExecuteNonQuery();
    }
    
    
    public IEnumerable<CategoryImage> GetCategoryPreview(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             SELECT ImageId 
                             From CategoryImage
                             where CategoryId=@CategoryId
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CategoryId", id);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return new CategoryImage()
            {
                ImageId = reader.GetInt32(0),
                CategoryId = id
            };
        }
    }

    public int DeleteCategory(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Delete From Category where Id = @Id";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
        return command.ExecuteNonQuery();
    }

    public int UpdateCategory(CategoryUpdateRequest categoryUpdateRequest)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Update Category 
                             Set Name = @Name, ParentCategoryId = @ParentId
                             Where Id = @Id;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", categoryUpdateRequest.Id);
        command.Parameters.AddWithValue("@Name", categoryUpdateRequest.Name);
        command.Parameters.AddWithValue("@ParentId", categoryUpdateRequest.ParentId);
        return command.ExecuteNonQuery();
    }
    
    
    public void DeleteCategoryPreview(int categoryId)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Delete From CategoryImage where CategoryId = @CategoryId";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@CategoryId", categoryId);
        command.ExecuteNonQuery();
    }
    
    public int IsCategoryPreviewExists(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = "Select count(*) from CategoryImage where CategoryId = @Id";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? reader.GetInt32(0) : 0;
    }

    
    
    public int UpdateCategoryPreview(CategoryPreviewUpdateRequest categoryPreviewUpdateRequest)
    {
        using var connection = new SqliteConnection(connectionString);
        const string query = """
                             Update CategoryImage 
                             Set ImageId = @ImageId
                             Where CategoryId = @CategoryId;
                             """;
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@ImageId", categoryPreviewUpdateRequest.FileId);
        command.Parameters.AddWithValue("@CategoryId", categoryPreviewUpdateRequest.CategoryId);
        return command.ExecuteNonQuery();
    }

}