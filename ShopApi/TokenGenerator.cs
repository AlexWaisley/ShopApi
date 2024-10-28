using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ShopApi;

public class TokenGenerator(ConfigurationManager configurationManager)
{
    private ConfigurationManager Config { get; } = configurationManager;

    public string GenerateToken(Guid userId, string email, bool isAdmin)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(Config["JwtSettings:Key"]!);

        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub,userId.ToString()),
            new (JwtRegisteredClaimNames.Email, email),
            new ("isAdmin", isAdmin.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = Config["JwtSettings:Issuer"],
            Audience = Config["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}