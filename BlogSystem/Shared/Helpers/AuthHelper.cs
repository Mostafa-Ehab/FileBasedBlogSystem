using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogSystem.Shared.Helpers;

public class AuthHelper
{
    private readonly int _hashWorkFactor = 10;
    private readonly SymmetricSecurityKey? _securityKey = null;
    public AuthHelper(IConfiguration configuration)
    {
        var jwtSecretKey = configuration["JWT_SecretKey"];
        if (string.IsNullOrWhiteSpace(jwtSecretKey))
        {
            throw new InvalidOperationException("JWT_SecretKey is not configured");
        }

        _hashWorkFactor = int.Parse(configuration["BC_HashWorkFactor"] ?? "10");
        _securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey));
    }

    public static string HashPassword(string password, int workFactor = 10) =>
        BC.EnhancedHashPassword(password, workFactor);

    public bool ValidatePassword(string password, string hashedPassword)
    {
        return BC.EnhancedVerify(password, hashedPassword);
    }

    public string HashPassword(string password)
    {
        return BC.EnhancedHashPassword(password, _hashWorkFactor);
    }

    // Generate JWT Token
    public string GenerateJWTToken(List<Claim> claims)
    {
        var jwtToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                _securityKey,
                SecurityAlgorithms.HmacSha256
        ));
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
