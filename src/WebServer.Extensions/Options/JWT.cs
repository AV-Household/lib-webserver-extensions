using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AV.Household.WebServer.Extensions.Options;

/// <summary>
///     Options for reading JSON web tokens
/// </summary>
public class Jwt
{
    /// <summary>
    ///     Token issuer
    /// </summary>
    public string Issuer { get; set; } = "AV.Household";

    /// <summary>
    ///     Security key for tokens
    /// </summary>
    public string SecurityKey { get; set; } = "Fake password";

    /// <summary>
    ///     Returns symmetric security key
    /// </summary>
    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.ASCII.GetBytes(SecurityKey));
}