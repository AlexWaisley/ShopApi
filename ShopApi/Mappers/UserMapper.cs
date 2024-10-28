using ShopApi.Dto;
using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Mappers;

public static class UserMapper
{
    public static UserDto MapToDto(this User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            Email = user.EmailInfo,
            Username = user.Name
        };
    }

    public static User MapToEntity(this UserDto userDto)
    {
        return new User()
        {
            Id = userDto.Id,
            EmailInfo = userDto.Email,
            Name = userDto.Username
        };
    }

    public static User MapToEntity(this UserRegisterRequest userRegisterRequest)
    {
        return new User()
        {
            Id = Guid.NewGuid(),
            EmailInfo = userRegisterRequest.EmailInfo,
            Name = userRegisterRequest.Name,
            Password = HashHelper.ComputeSha256Hash(userRegisterRequest.Password)
        };
    }
}