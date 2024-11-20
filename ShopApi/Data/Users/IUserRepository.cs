using ShopApi.Entity;
using ShopApi.FormModels;

namespace ShopApi.Data.Users;

public interface IUserRepository
{
    User? Login(UserLoginRequest userLoginRequest);
    int Register(UserRegisterRequest userRegisterRequestData);
    
    int UpdatePassword(UserPasswordUpdateRequest userPasswordUpdateRequest, Guid userId);
    int UpdateInfo(UserUpdateInfoRequest userUpdateInfoRequest, Guid userId);
    User? GetUserById(string userId);
}