using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Helpers;

public static class MilesSummaryHelper
{
    public static async Task<CustomerLoyaltyAccountSummaryResponse> BuildAccountSummaryAsync(
        CustomerLoyaltyAccount account,
        Guid tenantId,
        IMilesExpirationBatchRepository milesExpirationBatchRepository,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expiring30 = await milesExpirationBatchRepository.GetExpiringAmountAsync(account.Id, tenantId, today.AddDays(30), cancellationToken);
        var expiring60 = await milesExpirationBatchRepository.GetExpiringAmountAsync(account.Id, tenantId, today.AddDays(60), cancellationToken);
        var expiring90 = await milesExpirationBatchRepository.GetExpiringAmountAsync(account.Id, tenantId, today.AddDays(90), cancellationToken);

        return new CustomerLoyaltyAccountSummaryResponse(
            account.Id,
            account.LoyaltyProgramId,
            account.LoyaltyProgram.Name,
            account.AccountNumber,
            account.CurrentBalance,
            expiring30,
            expiring60,
            expiring90,
            account.AverageCostPerThousand,
            account.Status);
    }

    public static CustomerMilesSummaryResponse BuildCustomerSummary(
        Guid customerId,
        string customerName,
        IReadOnlyCollection<CustomerLoyaltyAccountSummaryResponse> accounts)
    {
        return new CustomerMilesSummaryResponse(
            customerId,
            customerName,
            accounts.Sum(x => x.CurrentBalance),
            accounts.Sum(x => x.ExpiringIn30Days),
            accounts.Sum(x => x.ExpiringIn60Days),
            accounts.Sum(x => x.ExpiringIn90Days),
            CalculateWeightedAverageCost(accounts),
            accounts);
    }

    public static decimal? CalculateWeightedAverageCost(IReadOnlyCollection<CustomerLoyaltyAccountSummaryResponse> accounts)
    {
        var weightedAccounts = accounts
            .Where(x => x.AverageCostPerThousand.HasValue && x.CurrentBalance > 0)
            .ToList();

        if (weightedAccounts.Count == 0)
            return null;

        var totalBalance = weightedAccounts.Sum(x => x.CurrentBalance);
        if (totalBalance == 0)
            return null;

        return weightedAccounts.Sum(x => x.AverageCostPerThousand!.Value * x.CurrentBalance) / totalBalance;
    }
}
