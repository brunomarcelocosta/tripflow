using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Repositories.CRM;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class TravelerRepositoryTests
{
    private static async Task<(Guid tenantId, Guid customerId)> SeedTenantAndCustomerAsync(string databaseName, string fullName = "Cliente Teste")
    {
        await using var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var tenant = new Tenant("Tenant LTDA", "Tenant", null, null, null, TenantStatus.Active, "system");
        seed.Tenants.Add(tenant);
        await seed.SaveChangesAsync();

        var customer = new Customer(tenant.Id, CustomerType.Person, fullName, null, null, null, null, null, "system");
        seed.Customers.Add(customer);
        await seed.SaveChangesAsync();

        return (tenant.Id, customer.Id);
    }

    [Fact]
    public async Task GetByCustomerAndTenantAsync_Should_Return_Traveler_For_Customer()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedTenantAndCustomerAsync(databaseName);
        Guid travelerId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var traveler = new Traveler(tenantId, customerId, "Viajante", null, "Brasileira", null, "BR111", null, null, "system");
            seed.Travelers.Add(traveler);
            await seed.SaveChangesAsync();
            travelerId = traveler.Id;
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TravelerRepository(ctx);

        var found = await repository.GetByCustomerAndTenantAsync(travelerId, customerId, tenantId);

        Assert.NotNull(found);
        Assert.Equal("BR111", found!.PassportNumber);
    }

    [Fact]
    public async Task GetByCustomerAndTenantAsync_Should_Return_Null_For_Other_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedTenantAndCustomerAsync(databaseName);
        Guid travelerId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var traveler = new Traveler(tenantId, customerId, "V", null, null, null, null, null, null, "system");
            seed.Travelers.Add(traveler);
            await seed.SaveChangesAsync();
            travelerId = traveler.Id;
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TravelerRepository(ctx);

        var found = await repository.GetByCustomerAndTenantAsync(travelerId, customerId, Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public async Task ExistsPassportNumberAsync_Should_Be_Tenant_Scoped()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedTenantAndCustomerAsync(databaseName);

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            seed.Travelers.Add(new Traveler(tenantId, customerId, "V", null, null, null, "PASSPORT999", null, null, "system"));
            await seed.SaveChangesAsync();
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TravelerRepository(ctx);

        Assert.True(await repository.ExistsPassportNumberAsync(tenantId, "PASSPORT999"));
        Assert.False(await repository.ExistsPassportNumberAsync(Guid.NewGuid(), "PASSPORT999"));
    }

    [Fact]
    public async Task CountByCustomersAsync_Should_Return_Counts_For_Given_Customers()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedTenantAndCustomerAsync(databaseName);

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            seed.Travelers.AddRange(
                new Traveler(tenantId, customerId, "V1", null, null, null, "P1", null, null, "system"),
                new Traveler(tenantId, customerId, "V2", null, null, null, "P2", null, null, "system"),
                new Traveler(tenantId, customerId, "V3", null, null, null, "P3", null, null, "system"));
            await seed.SaveChangesAsync();
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TravelerRepository(ctx);

        var counts = await repository.CountByCustomersAsync(tenantId, new[] { customerId });

        Assert.Equal(3, counts[customerId]);
    }
}
