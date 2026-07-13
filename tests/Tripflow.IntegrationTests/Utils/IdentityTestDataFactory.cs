using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Infra.Services;

namespace Tripflow.IntegrationTests.Utils;

internal static class IdentityTestDataFactory
{
    public static async Task SeedPermissionsAsync(TripflowDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Permissions.AnyAsync(cancellationToken))
            return;

        foreach (var code in TripflowDbSeedData.Permissions.All)
            context.Permissions.Add(new Permission(code, code));

        await context.SaveChangesAsync(cancellationToken);
    }

    public static async Task<Guid> SeedTenantAsync(
        TripflowDbContext context,
        string tradeName = "Tenant Teste",
        CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant(
            $"{tradeName} LTDA",
            tradeName,
            null,
            $"{tradeName.ToLowerInvariant()}@test.com",
            null,
            TenantStatus.Active,
            "system");

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }

    public static async Task<(Guid TenantId, Role Role)> SeedTenantWithConsultantRoleAsync(
        TripflowDbContext context,
        CancellationToken cancellationToken = default)
    {
        await SeedPermissionsAsync(context, cancellationToken);

        var tenantId = await SeedTenantAsync(context, cancellationToken: cancellationToken);

        var permissionRepository = new PermissionRepository(context);
        var provisioningService = new TenantRoleProvisioningService(context, permissionRepository);

        await provisioningService.ProvisionDefaultRolesAsync(tenantId, cancellationToken);

        var role = await context.Roles
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .FirstAsync(x => x.TenantId == tenantId && x.Name == TripflowDbSeedData.Roles.Consultant, cancellationToken);

        return (tenantId, role);
    }

    public static async Task<UserProfile> SeedUserProfileAsync(
        TripflowDbContext context,
        Guid tenantId,
        string email = "consultant@test.com",
        string identityProviderUserId = "keycloak-consultant",
        Role? role = null,
        CancellationToken cancellationToken = default)
    {
        role ??= await context.Roles
            .FirstAsync(x => x.TenantId == tenantId, cancellationToken);

        var profile = new UserProfile(
            tenantId,
            identityProviderUserId,
            "Consultor Teste",
            email,
            null,
            UserStatus.Active,
            "system");

        profile.AddRole(role.Id);

        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync(cancellationToken);

        return profile;
    }
}
