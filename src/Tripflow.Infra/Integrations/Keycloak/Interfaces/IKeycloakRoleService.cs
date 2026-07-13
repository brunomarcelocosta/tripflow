namespace Tripflow.Infra.Integrations.Keycloak.Interfaces;

public interface IKeycloakRoleService
{
    Task<bool> SyncRealmRolesAsync(string keycloakUserId, IReadOnlyList<string> roles, CancellationToken cancellationToken = default);
}
