using System.Globalization;
using Microsoft.Data.Sqlite;
using ShopApi.Entity;

namespace ShopApi.Data.Files;

public class FileRepository(string connectionString) : IFileRepository
{
    
    public IEnumerable<DbFile> GetFiles()
    {
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
        const string query = "Select Id, Size, Sha256, Name, CreatedAt, ContentType from Files where Id = @Id";
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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

}