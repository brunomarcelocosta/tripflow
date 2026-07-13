using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Infra.Integrations.Keycloak;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;
using Tripflow.Infra.Integrations.Keycloak.Services;

namespace Tripflow.Infra.IoC.DI;

public static class KeycloakDI
{
    public static IServiceCollection AddKeycloakIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KeycloakOptions>()
            .Bind(configuration.GetSection(KeycloakOptions.SectionName))
            .PostConfigure(options => options.PlatformTenantId ??= TripflowDbSeedData.PlatformTenantId)
            .Validate(x => !string.IsNullOrWhiteSpace(x.Authority), "Keycloak Authority is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Issuer), "Keycloak Issuer is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Audience), "Keycloak Audience is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.MetadataAddress), "Keycloak MetadataAddress is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Admin.AdminUrl), "Keycloak AdminUrl is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Admin.Realm), "Keycloak Realm is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Admin.ClientId), "Keycloak Admin ClientId is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Admin.ClientSecret), "Keycloak Admin ClientSecret is required.")
            .ValidateOnStart();

        services.AddHttpClient("KeycloakAdmin");

        services.AddScoped<IKeycloakAdminTokenProvider, KeycloakAdminTokenProvider>();
        services.AddScoped<IKeycloakUserService, KeycloakUserService>();
        services.AddScoped<IKeycloakRoleService, KeycloakRoleService>();

        return services;
    }

    public static IServiceCollection AddTripflowAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakSection = configuration.GetSection(KeycloakOptions.SectionName);

        services.Configure<KeycloakOptions>(keycloakSection);
        services.PostConfigure<KeycloakOptions>(options => options.PlatformTenantId ??= TripflowDbSeedData.PlatformTenantId);

        var keycloakOptions = keycloakSection.Get<KeycloakOptions>()
            ?? throw new InvalidOperationException("Keycloak configuration is missing.");
        keycloakOptions.PlatformTenantId ??= TripflowDbSeedData.PlatformTenantId;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakOptions.Authority;
                options.MetadataAddress = keycloakOptions.MetadataAddress;
                options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = keycloakOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = keycloakOptions.Audience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };
            });

        services.AddAuthorization();

        return services;
    }
}
