using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using ShopApi.Dto;
using ShopApi.FormModels;
using ShopApi.Mappers;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(Database database, TokenGenerator tokenGenerator) : ControllerBase
{
    [Authorize]
    [HttpGet("/user/cart/info")]
    public IActionResult GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.GetUserCart(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [HttpPost("/user/cart/item")]
    public IActionResult AddToCart([FromBody]CartItemDto item)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var cart = database.GetUserCart(id);
        if (cart is null)
            return NotFound();
        item.CartId = cart.Id;
        var result = database.AddToCart(item);
        if (result < 1)
            return NotFound();
        return Ok();
    }

    [Authorize]
    [HttpDelete("/user/cart/item/{id}")]
    public IActionResult RemoveFromCart(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var result = database.RemoveFromCart(int.Parse(id));
        if (result < 1)
            return NotFound();
        return Ok();
    }
    
    [Authorize]
    [HttpPost("/user/cart/update")]
    public IActionResult UpdateCart([FromBody]CartItemDto item)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var result = database.UpdateQuantity(item);
        if (result < 1)
            return NotFound();
        return Ok();
    }

    [HttpPost("/login")]
    public IActionResult Login([FromBody] UserLoginRequest userLoginRequest)
    {
        var result = database.Login(userLoginRequest);
        if (result is null) return NotFound();
        var refreshToken = tokenGenerator.GenerateRefreshToken();
        var added = database.AddRefreshToken(refreshToken,result.Id);
        if (added == 0)
            return BadRequest();
        
        var token = tokenGenerator.GenerateToken(result.Id,result.EmailInfo.Email,result.IsAdmin);
        Response.Cookies.Append("ultra-shop-token",token,new CookieOptions()
        {
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        Response.Cookies.Append("ultra-shop-token-refresh",refreshToken.ToString(),new CookieOptions()
        {
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
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
        var refreshToken = tokenGenerator.GenerateRefreshToken();
        var added = database.AddRefreshToken(refreshToken,result.Id);
        if (added == 0)
            return BadRequest();
        
        var token = tokenGenerator.GenerateToken(result.Id,result.EmailInfo.Email,result.IsAdmin);
        Response.Cookies.Append("ultra-shop-token",token,new CookieOptions()
        {
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        
        Response.Cookies.Append("ultra-shop-token-refresh",refreshToken.ToString(),new CookieOptions()
        {
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        return Ok(result.MapToDto());
    }
    
    [HttpPost("/refresh")]
    public IActionResult Refresh()
    {
        var token = Request.Cookies["ultra-shop-token-refresh"];
        if (token is null) return BadRequest();
        var userId = database.GetRefreshTokenValue(Guid.Parse(token));
        if (userId == null)
            return NotFound();

        var user = database.GetUserById(userId);
        if (user == null)
            return NotFound();

        var jwtToken = tokenGenerator.GenerateToken(user.Id, user.EmailInfo.Email, user.IsAdmin);
        Response.Cookies.Append("ultra-shop-token",jwtToken,new CookieOptions()
        {
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        
        return Ok();
    }

    [Authorize]
    [HttpPost("/verifyEmail")]
    public Results<Ok, NotFound> VerifyEmail(string email, string code)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("/user/password")]
    public IActionResult UpdatePassword([FromBody]UserPasswordUpdateRequest userPasswordUpdateRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.UpdatePassword(userPasswordUpdateRequest,id);
        if (result < 1)
            return NotFound();
        return Ok();
    }

    [Authorize]
    [HttpPost("/user/info")]
    public IActionResult UpdateUserInfo([FromBody]UserUpdateInfoRequest userUpdateInfoRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return BadRequest();
        var id = Guid.Parse(userId);
        var result = database.UpdateInfo(userUpdateInfoRequest,id);
        if (result < 1)
            return NotFound();
        return Ok();
    }
}