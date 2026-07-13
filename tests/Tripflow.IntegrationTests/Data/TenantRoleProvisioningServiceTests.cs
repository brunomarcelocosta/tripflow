using Microsoft.EntityFrameworkCore;
using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Infra.Services;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class TenantRoleProvisioningServiceTests
{
    [Fact]
    public async Task ProvisionDefaultRolesAsync_Should_Create_System_Roles_For_New_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        Guid tenantId;
        await using (var context = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await IdentityTestDataFactory.SeedPermissionsAsync(context);
            tenantId = await IdentityTestDataFactory.SeedTenantAsync(context);

            var service = new TenantRoleProvisioningService(
                context,
                new PermissionRepository(context));

            await service.ProvisionDefaultRolesAsync(tenantId);
        }

        await using var verifyContext = TripflowDbContextTestFactory.Create(databaseName: databaseName);

        var roles = await verifyContext.Roles
            .Where(x => x.TenantId == tenantId)
            .Select(x => x.Name)
            .ToListAsync();

        Assert.Contains(TripflowDbSeedData.Roles.TenantOwner, roles);
        Assert.Contains(TripflowDbSeedData.Roles.Consultant, roles);
        Assert.DoesNotContain(TripflowDbSeedData.Roles.PlatformAdmin, roles);
    }

    [Fact]
    public async Task ProvisionDefaultRolesAsync_Should_Not_Duplicate_Roles_On_Second_Call()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        Guid tenantId;
        await using (var context = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await IdentityTestDataFactory.SeedPermissionsAsync(context);
            tenantId = await IdentityTestDataFactory.SeedTenantAsync(context);

            var service = new TenantRoleProvisioningService(
                context,
                new PermissionRepository(context));

            await service.ProvisionDefaultRolesAsync(tenantId);
            await service.ProvisionDefaultRolesAsync(tenantId);
        }

        await using var verifyContext = TripflowDbContextTestFactory.Create(databaseName: databaseName);

        var roleCount = await verifyContext.Roles.CountAsync(x => x.TenantId == tenantId);

        Assert.Equal(DefaultTenantRoleDefinitions.Templates.Count, roleCount);
    }
}
