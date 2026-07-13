using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Infra.Data.Repositories.Pricing;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Data;

public class PaymentFeeRuleRepositoryTests
{
    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_All_Rules_Ordered_By_PaymentMethod_And_Installments()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Fee LTDA", "Fee", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            seedContext.PaymentFeeRules.AddRange(
                new PaymentFeeRule(tenantId, PaymentMethod.CreditCard, 6, 8m, true, "system"),
                new PaymentFeeRule(tenantId, PaymentMethod.CreditCard, 1, 3m, true, "system"),
                new PaymentFeeRule(tenantId, PaymentMethod.Pix, 1, 0m, true, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new PaymentFeeRuleRepository(context);

        var rules = await repository.GetByTenantIdAsync(tenantId);

        Assert.Equal(3, rules.Count);
        Assert.All(rules, r => Assert.Equal(tenantId, r.TenantId));
    }

    [Fact]
    public async Task GetByTenantPaymentMethodAndInstallmentsAsync_Should_Return_Matching_Rule()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Match LTDA", "Match", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            seedContext.PaymentFeeRules.AddRange(
                new PaymentFeeRule(tenantId, PaymentMethod.CreditCard, 1, 3m, true, "system"),
                new PaymentFeeRule(tenantId, PaymentMethod.CreditCard, 3, 5.5m, true, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new PaymentFeeRuleRepository(context);

        var match = await repository.GetByTenantPaymentMethodAndInstallmentsAsync(
            tenantId, PaymentMethod.CreditCard, 3);

        Assert.NotNull(match);
        Assert.Equal(5.5m, match!.FeePercentage);
    }

    [Fact]
    public async Task GetByTenantPaymentMethodAndInstallmentsAsync_Should_Return_Null_When_Not_Found()
    {
        await using var context = TripflowDbContextTestFactory.Create();
        var repository = new PaymentFeeRuleRepository(context);

        var match = await repository.GetByTenantPaymentMethodAndInstallmentsAsync(
            Guid.NewGuid(), PaymentMethod.Pix, 1);

        Assert.Null(match);
    }

    [Fact]
    public async Task GetTrackedByTenantPaymentMethodAndInstallmentsAsync_Should_Allow_Updating_Rule()
    {
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;

        await using (var seedContext = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Track LTDA", "Track", null, null, null, TenantStatus.Active, "system");
            seedContext.Tenants.Add(tenant);
            await seedContext.SaveChangesAsync();
            tenantId = tenant.Id;

            seedContext.PaymentFeeRules.Add(
                new PaymentFeeRule(tenantId, PaymentMethod.CreditCard, 2, 4m, true, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new PaymentFeeRuleRepository(context);

        var existing = await repository.GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
            tenantId, PaymentMethod.CreditCard, 2);
        Assert.NotNull(existing);

        existing!.Update(6.25m, false, "tester");
        await context.SaveChangesAsync();

        await using var verifyContext = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var verifyRepository = new PaymentFeeRuleRepository(verifyContext);
        var verify = await verifyRepository.GetByTenantPaymentMethodAndInstallmentsAsync(
            tenantId, PaymentMethod.CreditCard, 2);

        Assert.NotNull(verify);
        Assert.Equal(6.25m, verify!.FeePercentage);
        Assert.False(verify.IsActive);
    }

    [Fact]
    public async Task GetByTenantIdAsync_Should_Return_Only_Rules_For_Specified_Tenant()
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

            seedContext.PaymentFeeRules.AddRange(
                new PaymentFeeRule(tenantA, PaymentMethod.Pix, 1, 0m, true, "system"),
                new PaymentFeeRule(tenantA, PaymentMethod.CreditCard, 1, 3m, true, "system"),
                new PaymentFeeRule(tenantB, PaymentMethod.Pix, 1, 0m, true, "system"));
            await seedContext.SaveChangesAsync();
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var repository = new PaymentFeeRuleRepository(context);

        var rulesA = await repository.GetByTenantIdAsync(tenantA);
        var rulesB = await repository.GetByTenantIdAsync(tenantB);

        Assert.Equal(2, rulesA.Count);
        Assert.All(rulesA, r => Assert.Equal(tenantA, r.TenantId));
        Assert.Single(rulesB);
        Assert.Equal(tenantB, rulesB[0].TenantId);
    }
}
