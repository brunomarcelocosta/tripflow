using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Context;
using Tripflow.Infra.Data.Contexts;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class TripflowDbContextTenantIsolationTests
{
    [Fact]
    public async Task TenantContext_A_Should_Only_See_Customers_From_Tenant_A()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await TripflowDbContextTestFactory.SeedTwoTenantsWithCustomersAsync(seedContext);
        }

        var tenantContext = new TestTenantContext();

        await using (var resolveContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenants = await resolveContext.Tenants.OrderBy(x => x.LegalName).ToListAsync();
            tenantContext.SetTenant(tenants[0].Id);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var customers = await context.Customers.ToListAsync();

        Assert.Single(customers);
        Assert.Equal("Cliente A", customers[0].FullName);
        Assert.Equal(tenantContext.TenantId, customers[0].TenantId);
    }

    [Fact]
    public async Task TenantContext_B_Should_Only_See_Customers_From_Tenant_B()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await TripflowDbContextTestFactory.SeedTwoTenantsWithCustomersAsync(seedContext);
        }

        var tenantContext = new TestTenantContext();

        await using (var resolveContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenants = await resolveContext.Tenants.OrderBy(x => x.LegalName).ToListAsync();
            tenantContext.SetTenant(tenants[1].Id);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var customers = await context.Customers.ToListAsync();

        Assert.Single(customers);
        Assert.Equal("Cliente B", customers[0].FullName);
        Assert.Equal(tenantContext.TenantId, customers[0].TenantId);
    }

    [Fact]
    public async Task Without_TenantContext_Should_Not_Return_Tenant_Scoped_Data()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            await TripflowDbContextTestFactory.SeedTwoTenantsWithCustomersAsync(seedContext);
        }

        await using var context = TripflowDbContextTestFactory.Create(new TestTenantContext(), databaseName);

        var customers = await context.Customers.ToListAsync();

        Assert.Empty(customers);
    }

    [Fact]
    public async Task Global_Entities_Should_Remain_Queryable_Without_TenantContext()
    {
        await using var context = TripflowDbContextTestFactory.Create();

        context.PaymentProviders.Add(new PaymentProvider("manual", "Manual", PaymentProviderStatus.Active));
        context.SubscriptionPlans.Add(new SubscriptionPlan(
            "Starter",
            "Plano teste",
            10m,
            100m,
            1,
            10,
            SubscriptionPlanStatus.Active,
            "system"));

        await context.SaveChangesAsync();

        Assert.Single(await context.PaymentProviders.ToListAsync());
        Assert.Single(await context.SubscriptionPlans.ToListAsync());
    }

    [Fact]
    public async Task SaveChanges_Should_Reject_TenantEntity_With_Empty_TenantId()
    {
        var tenantContext = new TestTenantContext();
        tenantContext.SetTenant(Guid.NewGuid());

        await using var context = TripflowDbContextTestFactory.Create(tenantContext);

        var tenant = new Tenant("X", "X", null, null, null, TenantStatus.Active, "system");
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var invalidCustomer = new Customer(
            Guid.Empty,
            CustomerType.Person,
            "Invalid",
            null,
            null,
            null,
            null,
            null,
            "system");

        context.Customers.Add(invalidCustomer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChanges_Should_Reject_TenantEntity_With_Different_TenantId()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantA;
        Guid tenantB;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            (tenantA, tenantB) = await TripflowDbContextTestFactory.SeedTwoTenantsWithCustomersAsync(seedContext);
        }

        var tenantContext = new TestTenantContext();
        tenantContext.SetTenant(tenantA);

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var crossTenantCustomer = new Customer(
            tenantB,
            CustomerType.Person,
            "Cross Tenant",
            null,
            null,
            null,
            null,
            null,
            "system");

        context.Customers.Add(crossTenantCustomer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Soft_Delete_Filter_Should_Hide_Deleted_Customers()
    {
        var tenantContext = new TestTenantContext();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        context.Tenants.Add(new Tenant("Soft Delete", "Soft Delete", null, null, null, TenantStatus.Active, "system"));
        await context.SaveChangesAsync();

        var persistedTenantId = (await context.Tenants.SingleAsync()).Id;
        tenantContext.SetTenant(persistedTenantId);

        var customer = new Customer(
            persistedTenantId,
            CustomerType.Person,
            "Deleted Customer",
            null,
            null,
            null,
            null,
            null,
            "system");

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var entity = await context.Customers.SingleAsync();
        entity.SetDelete("system");
        await context.SaveChangesAsync();

        var visible = await context.Customers.ToListAsync();
        Assert.Empty(visible);
    }
}
