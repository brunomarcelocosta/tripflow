using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Data.Seeds;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class RoleRepositoryTests
{
    [Fact]
    public async Task GetAllByTenantAsync_Should_Return_Roles_With_Permissions()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            (tenantId, _) = await IdentityTestDataFactory.SeedTenantWithConsultantRoleAsync(seedContext);
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new RoleRepository(context);

        var roles = await repository.GetAllByTenantAsync(tenantId);

        Assert.Contains(roles, x => x.Name == TripflowDbSeedData.Roles.Consultant);
        var consultant = roles.First(x => x.Name == TripflowDbSeedData.Roles.Consultant);
        Assert.NotEmpty(consultant.RolePermissions);
        Assert.Contains(consultant.RolePermissions, rp => rp.Permission.Code == TripflowDbSeedData.Permissions.CustomersRead);
    }

    [Fact]
    public async Task GetByNamesAsync_Should_Return_Only_Roles_From_Specified_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            (tenantId, _) = await IdentityTestDataFactory.SeedTenantWithConsultantRoleAsync(seedContext);
            await IdentityTestDataFactory.SeedTenantAsync(seedContext, "Outro Tenant");
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new RoleRepository(context);

        var roles = await repository.GetByNamesAsync(
            tenantId,
            [TripflowDbSeedData.Roles.Consultant, TripflowDbSeedData.Roles.TenantOwner]);

        Assert.Equal(2, roles.Count);
        Assert.All(roles, x => Assert.Equal(tenantId, x.TenantId));
    }
}
