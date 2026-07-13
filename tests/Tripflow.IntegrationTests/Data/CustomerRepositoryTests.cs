using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Repositories.CRM;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class CustomerRepositoryTests
{
    [Fact]
    public async Task GetByIdAndTenantAsync_Should_Return_Customer_When_Tenant_Matches()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;
        Guid customerId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Tenant LTDA", "Tenant", null, null, null, TenantStatus.Active, "system");
            seed.Tenants.Add(tenant);
            await seed.SaveChangesAsync();
            tenantId = tenant.Id;

            var customer = new Customer(tenantId, CustomerType.Person, "João", "111", "joao@a.com", null, null, null, "system");
            seed.Customers.Add(customer);
            await seed.SaveChangesAsync();
            customerId = customer.Id;
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerRepository(ctx);

        var found = await repository.GetByIdAndTenantAsync(customerId, tenantId);

        Assert.NotNull(found);
        Assert.Equal("João", found!.FullName);
    }

    [Fact]
    public async Task GetByIdAndTenantAsync_Should_Return_Null_For_Other_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid otherTenant;
        Guid customerId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var (a, b) = await TripflowDbContextTestFactory.SeedTwoTenantsWithCustomersAsync(seed);
            otherTenant = b;
            var customerFromA = seed.Customers.First(c => c.TenantId == a);
            customerId = customerFromA.Id;
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerRepository(ctx);

        var found = await repository.GetByIdAndTenantAsync(customerId, otherTenant);

        Assert.Null(found);
    }

    [Fact]
    public async Task ExistsByDocumentNumberAsync_Should_Be_Tenant_Scoped()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantA;
        Guid tenantB;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var a = new Tenant("A LTDA", "A", null, null, null, TenantStatus.Active, "system");
            var b = new Tenant("B LTDA", "B", null, null, null, TenantStatus.Active, "system");
            seed.Tenants.AddRange(a, b);
            await seed.SaveChangesAsync();
            tenantA = a.Id;
            tenantB = b.Id;

            seed.Customers.Add(new Customer(tenantA, CustomerType.Person, "Cliente", "DOC999", null, null, null, null, "system"));
            await seed.SaveChangesAsync();
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerRepository(ctx);

        Assert.True(await repository.ExistsByDocumentNumberAsync(tenantA, "DOC999"));
        Assert.False(await repository.ExistsByDocumentNumberAsync(tenantB, "DOC999"));
    }

    [Fact]
    public async Task ExistsByEmailExceptIdAsync_Should_Ignore_SameCustomer()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;
        Guid customerId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("T LTDA", "T", null, null, null, TenantStatus.Active, "system");
            seed.Tenants.Add(tenant);
            await seed.SaveChangesAsync();
            tenantId = tenant.Id;

            var customer = new Customer(tenantId, CustomerType.Person, "X", null, "same@a.com", null, null, null, "system");
            seed.Customers.Add(customer);
            await seed.SaveChangesAsync();
            customerId = customer.Id;
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerRepository(ctx);

        Assert.True(await repository.ExistsByEmailAsync(tenantId, "same@a.com"));
        Assert.False(await repository.ExistsByEmailExceptIdAsync(tenantId, "same@a.com", customerId));
    }

    [Fact]
    public async Task SoftDelete_Should_HideCustomer_FromQueries()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;
        Guid customerId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("T LTDA", "T", null, null, null, TenantStatus.Active, "system");
            seed.Tenants.Add(tenant);
            await seed.SaveChangesAsync();
            tenantId = tenant.Id;

            var customer = new Customer(tenantId, CustomerType.Person, "DEL", null, null, null, null, null, "system");
            seed.Customers.Add(customer);
            await seed.SaveChangesAsync();
            customerId = customer.Id;
        }

        await using (var update = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var repo = new CustomerRepository(update);
            var entity = await repo.GetTrackedByIdAndTenantAsync(customerId, tenantId);
            entity!.SetDelete("system");
            await update.SaveChangesAsync();
        }

        await using var ctx = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new CustomerRepository(ctx);

        var found = await repository.GetByIdAndTenantAsync(customerId, tenantId);

        Assert.Null(found);
    }
}
