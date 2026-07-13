using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Services;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class AdminUserRepositoryTests
{
    [Fact]
    public async Task GetPagedForAdminAsync_Should_Return_Users_From_All_Tenants()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

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
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new UserProfileRepository(context);

        var result = await repository.GetPagedForAdminAsync(
            null,
            filter: null,
            orderBy: null,
            sortDesc: false,
            page: 1,
            pageSize: 10);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetPagedForAdminAsync_Should_Filter_By_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantA;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await IdentityTestDataFactory.SeedPermissionsAsync(seedContext);
            tenantA = await IdentityTestDataFactory.SeedTenantAsync(seedContext, "Tenant A");
            var tenantB = await IdentityTestDataFactory.SeedTenantAsync(seedContext, "Tenant B");

            var provisioning = new TenantRoleProvisioningService(
                seedContext,
                new PermissionRepository(seedContext));
            await provisioning.ProvisionDefaultRolesAsync(tenantA);
            await provisioning.ProvisionDefaultRolesAsync(tenantB);

            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantA, "usera@test.com", "keycloak-a");
            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantB, "userb@test.com", "keycloak-b");
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new UserProfileRepository(context);

        var result = await repository.GetPagedForAdminAsync(
            tenantA,
            filter: null,
            orderBy: null,
            sortDesc: false,
            page: 1,
            pageSize: 10);

        Assert.Equal(1, result.TotalItems);
        Assert.Equal("usera@test.com", result.Items.First().Email);
    }
}
