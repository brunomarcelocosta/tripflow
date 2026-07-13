using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;
using Tripflow.Infra.Integrations.Keycloak.Responses;

namespace Tripflow.Infra.Integrations.Keycloak.Services;

public sealed class KeycloakRoleService(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakOptions> options,
    IKeycloakAdminTokenProvider tokenProvider,
    ILogger<KeycloakRoleService> logger) : IKeycloakRoleService
{
    public async Task<bool> SyncRealmRolesAsync(
        string keycloakUserId,
        IReadOnlyList<string> roles,
        CancellationToken cancellationToken = default)
    {
        var admin = options.Value.Admin;

        if (string.IsNullOrWhiteSpace(admin.AdminUrl) ||
            string.IsNullOrWhiteSpace(admin.Realm))
        {
            logger.LogDebug("Keycloak Admin não configurado. Sincronização de roles ignorada.");
            return false;
        }

        try
        {
            var token = await tokenProvider.GetTokenAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(token))
                return false;

            await RemoveManagedRealmRolesAsync(
                token,
                keycloakUserId,
                admin.ManagedRolePrefix,
                cancellationToken);

            if (roles.Count == 0)
                return true;

            var roleRepresentations = await GetRealmRolesAsync(token, roles, cancellationToken);

            if (roleRepresentations.Count != roles.Count)
            {
                logger.LogWarning(
                    "Nem todas as roles foram encontradas no Keycloak. Solicitadas: {Requested}. Encontradas: {Found}",
                    string.Join(", ", roles),
                    string.Join(", ", roleRepresentations.Select(x => x.Name)));
            }

            if (roleRepresentations.Count > 0)
            {
                await AssignRolesToUserAsync(
                    token,
                    keycloakUserId,
                    roleRepresentations,
                    cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao sincronizar roles no Keycloak para usuário {UserId}", keycloakUserId);
            return false;
        }
    }

    private async Task<List<KeycloakRoleResponse>> GetRealmRolesAsync(
        string token,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken)
    {
        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var response = await client.GetAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/roles",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Falha ao listar roles do Keycloak: {StatusCode}", response.StatusCode);
            return [];
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var allRoles = JsonSerializer.Deserialize<List<KeycloakRoleResponse>>(json) ?? [];

        var roleNameSet = roleNames
            .Select(r => r.Trim().ToLowerInvariant())
            .ToHashSet();

        return allRoles
            .Where(r => r.Name != null && roleNameSet.Contains(r.Name.ToLowerInvariant()))
            .ToList();
    }

    private async Task RemoveManagedRealmRolesAsync(
        string token,
        string keycloakUserId,
        string managedPrefix,
        CancellationToken cancellationToken)
    {
        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var response = await client.GetAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}/role-mappings/realm",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var currentRoles = JsonSerializer.Deserialize<List<KeycloakRoleResponse>>(json) ?? [];

        var managedRoles = currentRoles
            .Where(x => !string.IsNullOrWhiteSpace(x.Name) &&
                        x.Name.StartsWith(managedPrefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (managedRoles.Count == 0)
            return;

        var content = new StringContent(
            JsonSerializer.Serialize(managedRoles),
            Encoding.UTF8,
            "application/json");

        var deleteResponse = await client.SendAsync(
            new HttpRequestMessage(
                HttpMethod.Delete,
                $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}/role-mappings/realm")
            {
                Content = content
            },
            cancellationToken);

        if (!deleteResponse.IsSuccessStatusCode)
        {
            var body = await deleteResponse.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Erro ao remover roles gerenciadas no Keycloak: {deleteResponse.StatusCode} - {body}");
        }
    }

    private async Task AssignRolesToUserAsync(
        string token,
        string keycloakUserId,
        List<KeycloakRoleResponse> roles,
        CancellationToken cancellationToken)
    {
        var admin = options.Value.Admin;
        var client = CreateAuthorizedClient(token);

        var content = new StringContent(
            JsonSerializer.Serialize(roles),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            $"{admin.AdminUrl.TrimEnd('/')}/admin/realms/{admin.Realm}/users/{keycloakUserId}/role-mappings/realm",
            content,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Keycloak retornou {response.StatusCode}: {body}");
        }
    }

    private HttpClient CreateAuthorizedClient(string token)
    {
        var client = httpClientFactory.CreateClient("KeycloakAdmin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
