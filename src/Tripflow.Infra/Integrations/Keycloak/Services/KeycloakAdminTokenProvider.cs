using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;

namespace Tripflow.Infra.Integrations.Keycloak.Services;

public sealed class KeycloakAdminTokenProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakOptions> options,
    ILogger<KeycloakAdminTokenProvider> logger) : IKeycloakAdminTokenProvider
{
    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var admin = options.Value.Admin;

        var client = httpClientFactory.CreateClient("KeycloakAdmin");

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{admin.AdminUrl.TrimEnd('/')}/realms/{admin.Realm}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = admin.ClientId,
                ["client_secret"] = admin.ClientSecret
            })
        };

        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogWarning(
                "Falha ao obter token admin do Keycloak. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.TryGetProperty("access_token", out var tokenProp)
            ? tokenProp.GetString()
            : null;
    }
}

