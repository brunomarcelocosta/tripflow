using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Repositories.Tenants;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class TenantBrandingRepositoryTests
{
    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Branding_When_Exists()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Branding LTDA", "Branding", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            seedContext.TenantBrandings.Add(
                new TenantBranding(tenantId, "https://cdn/test.png", "#000000", "#FFFFFF", "#1A1A1A", "Rodapé.", "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TenantBrandingRepository(context);

        var branding = await repository.GetByTenantIdAsync(tenantId);

        Assert.NotNull(branding);
        Assert.Equal(tenantId, branding!.TenantId);
        Assert.Equal("#000000", branding.PrimaryColor);
    }

    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Null_When_Not_Exists()
    {
        await using var context = TripflowDbContextTestFactory.Create();
        var repository = new TenantBrandingRepository(context);

        var branding = await repository.GetByTenantIdAsync(Guid.NewGuid());

        Assert.Null(branding);
    }

    [Fact]
    public async Task GetTrackedByTenantIdAsync_Should_Return_Tracked_Entity_For_Update()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Tracked LTDA", "Tracked", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            seedContext.TenantBrandings.Add(
                new TenantBranding(tenantId, null, "#111111", null, null, null, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TenantBrandingRepository(context);

        var branding = await repository.GetTrackedByTenantIdAsync(tenantId);
        Assert.NotNull(branding);

        branding!.UpdateColorsAndFooter("#222222", "#333333", "#444444", "Novo rodapé.", "tester");
        await context.SaveChangesAsync();

        await using var verifyContext = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var verifyRepository = new TenantBrandingRepository(verifyContext);
        var verify = await verifyRepository.GetByTenantIdAsync(tenantId);

        Assert.Equal("#222222", verify!.PrimaryColor);
        Assert.Equal("Novo rodapé.", verify.ProposalFooter);
    }

    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Only_Branding_For_Specified_Tenant()
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

            seedContext.TenantBrandings.AddRange(
                new TenantBranding(tenantA, null, "#AAAAAA", null, null, null, "system"),
                new TenantBranding(tenantB, null, "#BBBBBB", null, null, null, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new TenantBrandingRepository(context);

        var brandingA = await repository.GetByTenantIdAsync(tenantA);
        var brandingB = await repository.GetByTenantIdAsync(tenantB);

        Assert.NotNull(brandingA);
        Assert.NotNull(brandingB);
        Assert.Equal("#AAAAAA", brandingA!.PrimaryColor);
        Assert.Equal("#BBBBBB", brandingB!.PrimaryColor);
        Assert.NotEqual(brandingA.TenantId, brandingB.TenantId);
    }
}
