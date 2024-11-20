using Microsoft.Data.Sqlite;
using ShopApi.Entity;
using ShopApi.FormModels;
using ShopApi.Mappers;

namespace ShopApi.Data.Users;

public class UserRepository(string connectionString) : IUserRepository
{
    public User? Login(UserLoginRequest userLoginRequest)
    {
        var hashedPassword = HashHelper.ComputeSha256Hash(userLoginRequest.Password);
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
        using var connection = new SqliteConnection(connectionString);
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
}