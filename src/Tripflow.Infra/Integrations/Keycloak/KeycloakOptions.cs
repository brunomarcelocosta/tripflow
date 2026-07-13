namespace Tripflow.Infra.Integrations.Keycloak;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public string Authority { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;

    public string[] MasterAdminEmails { get; set; } = [];

    public KeycloakAdminOptions Admin { get; set; } = new();

    public Guid? PlatformTenantId { get; set; }
}

public class KeycloakAdminOptions
{
    public string AdminUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    public string ManagedRolePrefix { get; set; } = "tripflow.";
}

