using Microsoft.EntityFrameworkCore;
using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Services;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class TripflowDbContextUserProfileIsolationTests
{
    [Fact]
    public async Task TenantContext_Should_Only_See_UserProfiles_From_Current_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var tenantContext = new TestTenantContext();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await IdentityTestDataFactory.SeedPermissionsAsync(seedContext);
            var tenantA = await IdentityTestDataFactory.SeedTenantAsync(seedContext, "Tenant A");
            var tenantB = await IdentityTestDataFactory.SeedTenantAsync(seedContext, "Tenant B");
            var provisioning = new TenantRoleProvisioningService(
                seedContext,
                new PermissionRepository(seedContext));
            await provisioning.ProvisionDefaultRolesAsync(tenantA);
            await provisioning.ProvisionDefaultRolesAsync(tenantB);
            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantA, "usera@test.com", "keycloak-a");
            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantB, "userb@test.com", "keycloak-b");
            tenantContext.SetTenant(tenantA);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var profiles = await context.UserProfiles.ToListAsync();

        Assert.Single(profiles);
        Assert.Equal("usera@test.com", profiles[0].Email);
        Assert.Equal(tenantContext.TenantId, profiles[0].TenantId);
    }

    [Fact]
    public async Task Without_TenantContext_Should_Not_Return_UserProfiles()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var (tenantId, _) = await IdentityTestDataFactory.SeedTenantWithConsultantRoleAsync(seedContext);
            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantId);
        }

        await using var context = TripflowDbContextTestFactory.Create(new TestTenantContext(), databaseName);

        var profiles = await context.UserProfiles.ToListAsync();

        Assert.Empty(profiles);
    }
}
