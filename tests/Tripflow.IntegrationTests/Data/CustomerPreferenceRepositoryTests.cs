using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Repositories.CRM;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class CustomerPreferenceRepositoryTests
{
    private static async Task<(Guid tenantId, Guid customerId)> SeedAsync(string databaseName)
    {
        await using var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var tenant = new Tenant("Tenant LTDA", "Tenant", null, null, null, TenantStatus.Active, "system");
        seed.Tenants.Add(tenant);
        await seed.SaveChangesAsync();

        var customer = new Customer(tenant.Id, CustomerType.Person, "X", null, null, null, null, null, "system");
        seed.Customers.Add(customer);
        await seed.SaveChangesAsync();

        return (tenant.Id, customer.Id);
    }

    [Fact]
    public async Task GetByCustomerAndTenantAsync_Should_Return_Preference_When_Exists()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedAsync(databaseName);

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var pref = new CustomerPreference(tenantId, customerId, "system");
            pref.Update("LATAM", "Resort", "Janela", "Vegano", "Cruzeiros", "VIP", "system");
            seed.CustomerPreferences.Add(pref);
            await seed.SaveChangesAsync();
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerPreferenceRepository(ctx);

        var found = await repository.GetByCustomerAndTenantAsync(customerId, tenantId);

        Assert.NotNull(found);
        Assert.Equal("LATAM", found!.PreferredAirlines);
        Assert.Equal("VIP", found.GeneralNotes);
    }

    [Fact]
    public async Task GetByCustomerAndTenantAsync_Should_Return_Null_For_Other_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedAsync(databaseName);

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            seed.CustomerPreferences.Add(new CustomerPreference(tenantId, customerId, "system"));
            await seed.SaveChangesAsync();
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerPreferenceRepository(ctx);

        var found = await repository.GetByCustomerAndTenantAsync(customerId, Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public async Task GetTrackedByCustomerAndTenantAsync_Should_Allow_Update()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        var (tenantId, customerId) = await SeedAsync(databaseName);

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            seed.CustomerPreferences.Add(new CustomerPreference(tenantId, customerId, "system"));
            await seed.SaveChangesAsync();
        }

        await using (var update = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var repo = new CustomerPreferenceRepository(update);
            var tracked = await repo.GetTrackedByCustomerAndTenantAsync(customerId, tenantId);
            tracked!.Update("GOL", null, null, null, null, "Atualizado", "user@test");
            await update.SaveChangesAsync();
        }

        await using var verify = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var verifyRepo = new CustomerPreferenceRepository(verify);
        var found = await verifyRepo.GetByCustomerAndTenantAsync(customerId, tenantId);

        Assert.Equal("GOL", found!.PreferredAirlines);
        Assert.Equal("Atualizado", found.GeneralNotes);
        Assert.Equal("user@test", found.UpdatedBy);
    }
}
