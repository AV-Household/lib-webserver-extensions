using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AV.Household.WebServer.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AV.Household.WebServer.Testing.Driver;

/// <summary>
///     Driver to get default JWT tokens
/// </summary>
public class AuthFakerDriver
{
    /// <summary>
    ///     Default JWT Options for testing
    /// </summary>
    private static readonly Jwt JwtOptions = new()
    {
        Issuer = "AVHousehold",
        SecurityKey = "Pa$$w0rd_Pa$$w0rd_Pa$$w0rd"
    };

    /// <summary>
    ///     Get bearer JWT for specified user
    /// </summary>
    /// <param name="email">user email</param>
    /// <param name="household">user household</param>
    /// <param name="isAdult">is user adult or child</param>
    /// <returns>String representation of JWT</returns>
    public string GetBearer(string email, int household, bool isAdult)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            new Claim("Household", household.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, isAdult ? "Adult" : "Child")
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = JwtOptions.Issuer,
            Audience = $"*.{JwtOptions.Issuer}",
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(10),
            Subject = subject,
            SigningCredentials =
                new SigningCredentials(JwtOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}