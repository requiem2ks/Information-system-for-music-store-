using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MuzShop.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MuzShop.Services;
public class AuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim("userid", user.Userid.ToString()),
        new Claim(ClaimTypes.Role, user.Role.Rolename)
    };

        if (user.Clientid != null)
            claims.Add(new Claim("Clientid", user.Clientid.ToString()!));

        if (user.Employeeid != null)
            claims.Add(new Claim("Employeeid", user.Employeeid.ToString()!));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
