using System.Text.Json.Serialization;

namespace Tripflow.Infra.Integrations.Keycloak.Services;

public sealed partial class KeycloakUserService
{
    public sealed class KeycloakUserResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("createdTimestamp")]
        public long? CreatedTimestamp { get; set; }
    }
}