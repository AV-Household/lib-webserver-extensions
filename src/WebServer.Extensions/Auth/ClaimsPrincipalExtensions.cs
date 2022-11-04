using System.Security.Claims;

namespace AV.Household.WebServer.Extensions.Auth;

/// <summary>
///     Extension methods for authorized user
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public const string AdultRoleName = "Adult";

    /// <summary>
    ///     Get household id from user claim
    /// </summary>
    /// <param name="user">User principal</param>
    /// <returns>Household id or null</returns>
    public static int? GetHousehold(this ClaimsPrincipal user)
    {
        var claimValue = user?.Claims?.SingleOrDefault(claim => claim.Type == "Household")?.Value;

        if (claimValue is null)
            return null;

        if (!int.TryParse(claimValue, out var result))
            return null;

        return result;
    }

    /// <summary>
    ///     True if user has adult role
    /// </summary>
    /// <param name="user">User principal</param>
    /// <returns></returns>
    public static bool IsAdult(this ClaimsPrincipal user) => user.IsInRole(AdultRoleName);
}