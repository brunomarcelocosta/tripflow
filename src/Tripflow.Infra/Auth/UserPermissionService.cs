using Microsoft.Extensions.Options;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Integrations.Keycloak;

namespace Tripflow.Infra.Auth;

public sealed class UserPermissionService(
    IUserContext userContext,
    IUserProfileRepository userProfileRepository,
    IPermissionRepository permissionRepository,
    IOptions<KeycloakOptions> keycloakOptions) : IUserPermissionService
{
    public async Task<bool> HasPermissionAsync(
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetCurrentUserPermissionsAsync(cancellationToken);

        return permissions.Contains(permissionCode, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<string>> GetCurrentUserPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated ||
            string.IsNullOrWhiteSpace(userContext.IdentityProviderUserId))
        {
            return [];
        }

        if (IsMasterAdmin(userContext.Email))
        {
            var all = await permissionRepository.GetAllAsync(cancellationToken);
            return all.Select(x => x.Code).ToArray();
        }

        var profile = await userProfileRepository.GetByIdentityProviderUserIdAsync(
            userContext.IdentityProviderUserId,
            cancellationToken);

        if (profile is null)
            return [];

        return profile.UserRoles
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Code)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private bool IsMasterAdmin(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var masterEmails = keycloakOptions.Value.MasterAdminEmails ?? [];

        return masterEmails.Contains(email.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
