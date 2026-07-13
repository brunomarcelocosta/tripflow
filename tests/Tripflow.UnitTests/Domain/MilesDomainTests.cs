using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.DomainModels;

public class MilesDomainTests
{
    [Fact]
    public void MilesTransaction_CalculateTotalCost_Should_ComputeCorrectly()
    {
        var total = MilesTransaction.CalculateTotalCost(80000, 20m);
        Assert.Equal(1600m, total);
    }

    [Fact]
    public void CustomerLoyaltyAccount_DebitMiles_Should_PreventNegativeBalance()
    {
        var account = new CustomerLoyaltyAccount(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "123", 1000, 20m, null,
            LoyaltyAccountStatus.Active, "system");

        account.DebitMiles(400, "system");
        Assert.Equal(600, account.CurrentBalance);

        Assert.Throws<InvalidOperationException>(() => account.DebitMiles(700, "system"));
    }

    [Fact]
    public void MilesExpirationBatch_Consume_Should_UseFifoRemainingAmount()
    {
        var batch = new MilesExpirationBatch(
            Guid.NewGuid(), Guid.NewGuid(), 1000, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            MilesExpirationStatus.Pending, "system");

        var consumed = batch.Consume(400, "system");
        Assert.Equal(400, consumed);
        Assert.Equal(600, batch.RemainingAmount);
        Assert.Equal(MilesExpirationStatus.Pending, batch.Status);

        consumed = batch.Consume(600, "system");
        Assert.Equal(600, consumed);
        Assert.Equal(0, batch.RemainingAmount);
        Assert.Equal(MilesExpirationStatus.Used, batch.Status);
    }

    [Fact]
    public void LoyaltyProgram_ActivateAndInactivate_Should_UpdateStatus()
    {
        var program = new LoyaltyProgram("Smiles", "BR", "GOL", LoyaltyProgramStatus.Inactive, "system");
        program.Activate("admin");
        Assert.Equal(LoyaltyProgramStatus.Active, program.Status);

        program.Inactivate("admin");
        Assert.Equal(LoyaltyProgramStatus.Inactive, program.Status);
    }

    [Fact]
    public void TenantUsage_LimitReachedAndRemaining_Should_Work()
    {
        var usage = new TenantUsage(Guid.NewGuid(), "quotes", 2026, 5, 9, 10);
        Assert.False(usage.LimitReached());
        Assert.Equal(1, usage.Remaining());

        usage.Increment();
        Assert.True(usage.LimitReached());
        Assert.Equal(0, usage.Remaining());
    }

    [Fact]
    public void SubscriptionPlan_CanBeAssigned_Should_BlockDeprecated()
    {
        var plan = new SubscriptionPlan("Legacy", null, 0, 0, 5, 100, SubscriptionPlanStatus.Deprecated, "system");
        Assert.False(plan.CanBeAssigned());
    }

    [Fact]
    public void Lead_MarkAsConverted_Should_SetFlags()
    {
        var lead = new Lead("Empresa", "Responsável", "lead@test.com", null, null, null, "public");
        var tenantId = Guid.NewGuid();

        lead.MarkAsConverted(tenantId, "admin");

        Assert.True(lead.ConvertedToTenant);
        Assert.Equal(tenantId, lead.ConvertedTenantId);
    }
}
