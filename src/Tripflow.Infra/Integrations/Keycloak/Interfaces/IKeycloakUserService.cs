namespace Tripflow.Infra.Integrations.Keycloak.Interfaces;

public sealed record KeycloakUserDto(
    string Id,
    string? Email,
    string? Username,
    string? FirstName,
    string? LastName,
    bool Enabled,
    long? CreatedTimestamp);

public interface IKeycloakUserService
{
    Task<string?> CreateUserAsync(
        string email,
        string fullName,
        bool enabled = true,
        bool emailVerified = false,
        CancellationToken cancellationToken = default);

    Task<string?> GetUserIdByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<KeycloakUserDto>> GetUsersAsync(
        string? search = null,
        int first = 0,
        int max = 100,
        CancellationToken cancellationToken = default);

    Task<KeycloakUserDto?> GetUserByIdAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateUserAsync(
        string keycloakUserId,
        string email,
        string fullName,
        bool enabled,
        CancellationToken cancellationToken = default);

    Task<bool> SetPasswordAsync(
        string keycloakUserId,
        string password,
        bool temporary = false,
        CancellationToken cancellationToken = default);

    Task<bool> EnableUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default);

    Task<bool> DisableUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default);

    Task<bool> SendResetPasswordEmailAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default);
}
