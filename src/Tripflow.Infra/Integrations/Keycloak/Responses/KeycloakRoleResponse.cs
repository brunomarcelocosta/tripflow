using System.Text.Json.Serialization;

namespace Tripflow.Infra.Integrations.Keycloak.Responses;

public sealed class KeycloakRoleResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}