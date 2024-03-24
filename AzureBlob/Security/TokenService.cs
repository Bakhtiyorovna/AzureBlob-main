using AzureBlob1.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AzureBlob1.Security;

public class TokenService : ITokenService
 {    
    
    private readonly IConfiguration _config;
    public TokenService(IConfiguration configuration)
    {
        _config = configuration.GetSection("Jwt");
    }


    public string GenerateToken(Admin admin)
    {
        var identityClaims = new Claim[]
        {
            new Claim("Id", admin.Id.ToString()),
            new Claim("Name", admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]!));
        var keyCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        int expiresHours = int.Parse(_config["Lifetime"]!);
        var token = new JwtSecurityToken(
            issuer: _config["Issuer"],
            audience: _config["Audience"],
            claims: identityClaims,
            expires: DateTime.Now.AddHours(expiresHours),
            signingCredentials: keyCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
