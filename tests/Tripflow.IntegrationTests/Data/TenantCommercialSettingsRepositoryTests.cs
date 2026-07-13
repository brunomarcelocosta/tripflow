using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Repositories.Tenants;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class TenantCommercialSettingsRepositoryTests
{
    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Settings_When_Exists()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Comercial LTDA", "Comercial", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            var seed = new TenantCommercialSettings(tenantId, "system");
            seed.UpdateCommercialData(
                "comercial@empresa.com",
                "11999990000",
                "11988887777",
                "@empresa",
                "https://empresa.com",
                "Termos.",
                "Notas.",
                48,
                "system");

            seedContext.TenantCommercialSettings.Add(seed);
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TenantCommercialSettingsRepository(context);

        var settings = await repository.GetByTenantIdAsync(tenantId);

        Assert.NotNull(settings);
        Assert.Equal(tenantId, settings!.TenantId);
        Assert.Equal("comercial@empresa.com", settings.CommercialEmail);
        Assert.Equal(48, settings.DefaultProposalExpirationHours);
    }

    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Null_When_Not_Exists()
    {
        await using var context = TripflowDbContextTestFactory.Create();
        var repository = new TenantCommercialSettingsRepository(context);

        var settings = await repository.GetByTenantIdAsync(Guid.NewGuid());

        Assert.Null(settings);
    }

    [Fact]
    public async Task GetTrackedByTenantIdAsync_Should_Allow_Financial_Update()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Fin LTDA", "Fin", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            seedContext.TenantCommercialSettings.Add(new TenantCommercialSettings(tenantId, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TenantCommercialSettingsRepository(context);

        var settings = await repository.GetTrackedByTenantIdAsync(tenantId);
        Assert.NotNull(settings);

        settings!.UpdateFinancialDefaults(200m, null, 7.5m, "tester");
        await context.SaveChangesAsync();

        await using var verifyContext = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var verifyRepository = new TenantCommercialSettingsRepository(verifyContext);
        var verify = await verifyRepository.GetByTenantIdAsync(tenantId);

        Assert.Equal(200m, verify!.DefaultProfitAmount);
        Assert.Null(verify.DefaultProfitPercentage);
        Assert.Equal(7.5m, verify.DefaultPixDiscountPercentage);
    }

    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Only_Settings_For_Specified_Tenant()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantA;
        Guid tenantB;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var a = new Tenant("Tenant A LTDA", "Tenant A", null, null, null, TenantStatus.Active, "system");
            var b = new Tenant("Tenant B LTDA", "Tenant B", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.AddRange(a, b);
            await seedContext.SaveChangesAsync();
            tenantA = a.Id;
            tenantB = b.Id;

            var sA = new TenantCommercialSettings(tenantA, "system");
            sA.UpdateCommercialData("a@a.com", null, null, null, null, null, null, 24, "system");
            var sB = new TenantCommercialSettings(tenantB, "system");
            sB.UpdateCommercialData("b@b.com", null, null, null, null, null, null, 48, "system");

            seedContext.TenantCommercialSettings.AddRange(sA, sB);
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TenantCommercialSettingsRepository(context);

        var settingsA = await repository.GetByTenantIdAsync(tenantA);
        var settingsB = await repository.GetByTenantIdAsync(tenantB);

        Assert.Equal("a@a.com", settingsA!.CommercialEmail);
        Assert.Equal(24, settingsA.DefaultProposalExpirationHours);
        Assert.Equal("b@b.com", settingsB!.CommercialEmail);
        Assert.Equal(48, settingsB.DefaultProposalExpirationHours);
    }
}
