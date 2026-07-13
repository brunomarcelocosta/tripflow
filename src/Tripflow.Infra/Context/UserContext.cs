using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Infra.Auth;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? IdentityProviderUserId => GetClaimValue(
        ClaimTypes.NameIdentifier,
        "sub");

    public string? Email => GetClaimValue(
        ClaimTypes.Email,
        "email",
        "preferred_username",
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

    public string? Name => GetClaimValue(
        ClaimTypes.Name,
        "name",
        "preferred_username");

    public IReadOnlyList<string> Roles => GetRoles();

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private string? GetClaimValue(params string[] claimTypes)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user is null)
            return null;

        foreach (var claimType in claimTypes)
        {
            var claim = user.FindFirst(claimType);

            if (!string.IsNullOrWhiteSpace(claim?.Value))
                return claim.Value;
        }

        return null;
    }

    private IReadOnlyList<string> GetRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return [];

        var roles = new List<string>();

        roles.AddRange(user.FindAll(ClaimTypes.Role).Select(x => x.Value));
        roles.AddRange(user.FindAll("role").Select(x => x.Value));
        roles.AddRange(user.FindAll("roles").Select(x => x.Value));

        var realmAccess = user.FindFirst("realm_access")?.Value;

        if (!string.IsNullOrWhiteSpace(realmAccess))
        {
            try
            {
                var realm = JsonSerializer.Deserialize<RealmAccessClaim>(
                    realmAccess,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (realm?.Roles is { Length: > 0 })
                    roles.AddRange(realm.Roles);
            }
            catch
            {
                // Ignora formato inválido de realm_access.
            }
        }

        return roles
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private sealed record RealmAccessClaim(string[] Roles);
}