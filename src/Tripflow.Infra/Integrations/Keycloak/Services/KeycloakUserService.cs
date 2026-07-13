using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;

namespace Tripflow.Infra.Integrations.Keycloak.Services;

public sealed partial class KeycloakUserService(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakOptions> options,
    IKeycloakAdminTokenProvider tokenProvider,
    ILogger<KeycloakUserService> logger) : IKeycloakUserService
{
    public async Task<string?> CreateUserAsync(
        string email,
        string fullName,
        bool enabled = true,
        bool emailVerified = false,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var name = SplitName(fullName);

        var payload = new
        {
            username = email,
            email,
            firstName = name.FirstName,
            lastName = name.LastName,
            enabled,
            emailVerified
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users",
            content,
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            logger.LogInformation("Usuário já existe no Keycloak: {Email}", email);
            return await GetUserIdByEmailAsync(email, cancellationToken);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao criar usuário no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return null;
        }

        if (response.Headers.Location is null)
            return await GetUserIdByEmailAsync(email, cancellationToken);

        return response.Headers.Location.ToString().Split('/').LastOrDefault();
    }

    public async Task<string?> GetUserIdByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var response = await client.GetAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users?email={Uri.EscapeDataString(email)}&exact=true",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao buscar usuário por e-mail no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var users = JsonSerializer.Deserialize<List<KeycloakUserResponse>>(
            json,
            JsonOptions()) ?? [];

        return users.FirstOrDefault()?.Id;
    }

    public async Task<IReadOnlyList<KeycloakUserDto>> GetUsersAsync(
        string? search = null,
        int first = 0,
        int max = 100,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return [];

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var query = $"first={first}&max={max}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search.Trim())}";

        var response = await client.GetAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users?{query}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao listar usuários no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return [];
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var users = JsonSerializer.Deserialize<List<KeycloakUserResponse>>(
            json,
            JsonOptions()) ?? [];

        return users
            .Where(x => !string.IsNullOrWhiteSpace(x.Id))
            .Select(MapUserDto)
            .ToList();
    }

    public async Task<KeycloakUserDto?> GetUserByIdAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var response = await client.GetAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao buscar usuário no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var user = JsonSerializer.Deserialize<KeycloakUserResponse>(json, JsonOptions());

        return user is null || string.IsNullOrWhiteSpace(user.Id) ? null : MapUserDto(user);
    }

    public async Task<bool> UpdateUserAsync(
        string keycloakUserId,
        string email,
        string fullName,
        bool enabled,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);
        var name = SplitName(fullName);

        var payload = new
        {
            username = email,
            email,
            firstName = name.FirstName,
            lastName = name.LastName,
            enabled
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}",
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao atualizar usuário no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return false;
        }

        return true;
    }

    public async Task<bool> SetPasswordAsync(
        string keycloakUserId,
        string password,
        bool temporary = false,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var payload = new
        {
            type = "password",
            value = password,
            temporary
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}/reset-password",
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao definir senha no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return false;
        }

        return true;
    }

    public async Task<bool> EnableUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default)
    {
        return await UpdateUserEnabledAsync(keycloakUserId, true, cancellationToken);
    }

    public async Task<bool> DisableUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default)
    {
        return await UpdateUserEnabledAsync(keycloakUserId, false, cancellationToken);
    }

    public async Task<bool> SendResetPasswordEmailAsync(
        string keycloakUserId,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var actions = new[] { "UPDATE_PASSWORD", "VERIFY_EMAIL" };

        var content = new StringContent(
            JsonSerializer.Serialize(actions),
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}/execute-actions-email",
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao enviar e-mail de reset pelo Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return false;
        }

        return true;
    }

    private async Task<bool> UpdateUserEnabledAsync(
        string keycloakUserId,
        bool enabled,
        CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var payload = new { enabled };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PutAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}",
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Erro ao atualizar status do usuário no Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return false;
        }

        return true;
    }

    private HttpClient CreateAuthorizedClient(string token)
    {
        var client = httpClientFactory.CreateClient("KeycloakAdmin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static (string FirstName, string LastName) SplitName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return ("", "");

        if (parts.Length == 1)
            return (parts[0], "");

        return (parts[0], string.Join(' ', parts.Skip(1)));
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private static KeycloakUserDto MapUserDto(KeycloakUserResponse user)
    {
        return new KeycloakUserDto(
            user.Id!,
            user.Email,
            user.Username,
            user.FirstName,
            user.LastName,
            user.Enabled,
            user.CreatedTimestamp);
    }
}