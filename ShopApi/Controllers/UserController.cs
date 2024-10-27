using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;

namespace ShopApi.Controllers;

public class UserController
{
    [HttpGet("cart")]
    //user from cookie
    public async Task<CartDto> GetCart()
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("cart")]
    //user from cookie
    public async Task<CartDto> AddToCart(CartItemDto item)
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete("cart/{id}")]
    //user from cookie
    public async Task<CartDto> RemoveFromCart(string id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("/login")]
    public Ok<UserDto> Login(string email, string password)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("/register")]
    public async Task<UserDto> Register(string email, string password)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("/verifyEmail")]
    public Results<Ok,NotFound> VerifyEmail(string email, string code)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("/updatePassword")]
    public Results<Ok,NotFound> UpdatePassword(string password)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("/updateUserInfo")]
    public Results<Ok,NotFound> UpdateUserInfo(string email, string name)
    {
        throw new NotImplementedException();
    }
    
    
    
}