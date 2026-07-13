using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Helpers;

public class MilesSummaryHelperTests
{
    [Fact]
    public void BuildCustomerSummary_Should_AggregateBalancesAndExpiringWindows()
    {
        var accounts = new List<CustomerLoyaltyAccountSummaryResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Smiles", "1", 1000, 100, 200, 300, 20m, LoyaltyAccountStatus.Active),
            new(Guid.NewGuid(), Guid.NewGuid(), "Latam", "2", 500, 50, 80, 120, 30m, LoyaltyAccountStatus.Active)
        };

        var summary = MilesSummaryHelper.BuildCustomerSummary(Guid.NewGuid(), "Cliente Demo", accounts);

        Assert.Equal(1500, summary.TotalBalance);
        Assert.Equal(150, summary.TotalExpiringIn30Days);
        Assert.Equal(280, summary.TotalExpiringIn60Days);
        Assert.Equal(420, summary.TotalExpiringIn90Days);
        Assert.Equal(23.33m, Math.Round(summary.AverageCostPerThousand!.Value, 2));
    }

    [Fact]
    public void CalculateWeightedAverageCost_Should_ReturnNull_WhenNoCostData()
    {
        var accounts = new List<CustomerLoyaltyAccountSummaryResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Smiles", null, 1000, 0, 0, 0, null, LoyaltyAccountStatus.Active)
        };

        Assert.Null(MilesSummaryHelper.CalculateWeightedAverageCost(accounts));
    }
}
