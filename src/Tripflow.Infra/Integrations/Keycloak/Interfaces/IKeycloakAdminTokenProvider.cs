namespace Tripflow.Infra.Integrations.Keycloak.Interfaces;

public interface IKeycloakAdminTokenProvider
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}

