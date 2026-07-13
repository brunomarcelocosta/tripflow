using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.IntegrationTests.Utils;

internal sealed class TestTenantContext : ITenantContext, ITenantContextSetter
{
    public Guid? TenantId { get; private set; }
    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(Guid tenantId) => TenantId = tenantId;
    public void Clear() => TenantId = null;
}

internal static class TripflowDbContextTestFactory
{
    public static string CreateDatabaseName() => Guid.NewGuid().ToString();

    public static TripflowDbContext Create(ITenantContext? tenantContext = null, string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<TripflowDbContext>()
            .UseInMemoryDatabase(databaseName ?? CreateDatabaseName())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new TripflowDbContext(options, tenantContext);
    }

    public static async Task<(Guid TenantA, Guid TenantB)> SeedTwoTenantsWithCustomersAsync(
        TripflowDbContext context,
        CancellationToken cancellationToken = default)
    {
        context.Tenants.AddRange(
            new Tenant("Tenant A LTDA", "Tenant A", null, "a@test.com", null, TenantStatus.Active, "system"),
            new Tenant("Tenant B LTDA", "Tenant B", null, "b@test.com", null, TenantStatus.Active, "system"));

        await context.SaveChangesAsync(cancellationToken);

        var tenants = await context.Tenants.OrderBy(x => x.LegalName).ToListAsync(cancellationToken);
        var idA = tenants[0].Id;
        var idB = tenants[1].Id;

        context.Customers.AddRange(
            new Customer(idA, CustomerType.Person, "Cliente A", null, "a@customer.com", null, null, null, "system"),
            new Customer(idB, CustomerType.Person, "Cliente B", null, "b@customer.com", null, null, null, "system"));

        await context.SaveChangesAsync(cancellationToken);

        return (idA, idB);
    }
}
