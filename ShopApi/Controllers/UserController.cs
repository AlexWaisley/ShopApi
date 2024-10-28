using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dto;
using ShopApi.FormModels;
using ShopApi.Mappers;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(Database database, TokenGenerator tokenGenerator) : ControllerBase
{
    [HttpGet("cart")]
    public async Task<CartDto> GetCart()
    {
        throw new NotImplementedException();
    }

    [HttpPost("cart")]
    public async Task<CartDto> AddToCart(CartItemDto item)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("cart/{id}")]
    public async Task<CartDto> RemoveFromCart(string id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("/login")]
    public IActionResult Login([FromBody] UserLoginRequest userLoginRequest)
    {
        var result = database.Login(userLoginRequest);
        if (result is null) return NotFound();
        
        var token = tokenGenerator.GenerateToken(result.Id,result.EmailInfo.Email,result.IsAdmin);
        Response.Cookies.Append("jwt",token,new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        return Ok(result.MapToDto());
    }

    [HttpPost("/register")]
    public IActionResult Register([FromBody] UserRegisterRequest userRegisterRequest)
    {
        var registerStatus = database.Register(userRegisterRequest);
        if (registerStatus == 0) return BadRequest();
        var result = database.Login(userRegisterRequest.MapToLoginRequest());
        if (result is null) return NotFound();
        
        var token = tokenGenerator.GenerateToken(result.Id,result.EmailInfo.Email,result.IsAdmin);
        Response.Cookies.Append("jwt",token,new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        return Ok(result.MapToDto());

    }

    [HttpPost("/verifyEmail")]
    public Results<Ok, NotFound> VerifyEmail(string email, string code)
    {
        throw new NotImplementedException();
    }

    [HttpPost("/updatePassword")]
    public Results<Ok, NotFound> UpdatePassword(UserPasswordUpdateRequest userPasswordUpdateRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPost("/updateUserInfo")]
    public Results<Ok, NotFound> UpdateUserInfo(UserUpdateInfoRequest userUpdateInfoRequest)
    {
        throw new NotImplementedException();
    }
}