using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Services;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class UserProfileRepositoryTests
{
    [Fact]
    public async Task GetByIdentityProviderUserIdAsync_Should_Include_Roles_And_Permissions()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var (tenantId, role) = await IdentityTestDataFactory.SeedTenantWithConsultantRoleAsync(seedContext);
            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantId, role: role);
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new UserProfileRepository(context);

        var profile = await repository.GetByIdentityProviderUserIdAsync("keycloak-consultant");

        Assert.NotNull(profile);
        Assert.Single(profile!.UserRoles);
        Assert.Equal("Consultant", profile.UserRoles[0].Role.Name);
        Assert.NotEmpty(profile.UserRoles[0].Role.RolePermissions);
    }

    [Fact]
    public async Task ExistsByEmailInTenantAsync_Should_Return_True_When_Email_Exists()
    {
        var tenantContext = new TestTenantContext();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await IdentityTestDataFactory.SeedPermissionsAsync(seedContext);
            var tenantId = await IdentityTestDataFactory.SeedTenantAsync(seedContext);
            tenantContext.SetTenant(tenantId);
            var provisioning = new TenantRoleProvisioningService(
                seedContext,
                new PermissionRepository(seedContext));
            await provisioning.ProvisionDefaultRolesAsync(tenantId);
            await IdentityTestDataFactory.SeedUserProfileAsync(seedContext, tenantId, "existente@test.com");
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);
        var repository = new UserProfileRepository(context);

        var exists = await repository.ExistsByEmailInTenantAsync(tenantContext.TenantId!.Value, "existente@test.com");

        Assert.True(exists);
    }

    [Fact]
    public async Task GetByIdInTenantAsync_Should_Return_Null_For_User_From_Other_Tenant()
    {
        var tenantContext = new TestTenantContext();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid userFromOtherTenant;

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

            var userB = await IdentityTestDataFactory.SeedUserProfileAsync(
                seedContext,
                tenantB,
                "userb@test.com",
                "keycloak-b");

            tenantContext.SetTenant(tenantA);
            userFromOtherTenant = userB.Id;
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);
        var repository = new UserProfileRepository(context);

        var profile = await repository.GetByIdInTenantAsync(tenantContext.TenantId!.Value, userFromOtherTenant);

        Assert.Null(profile);
    }
}
