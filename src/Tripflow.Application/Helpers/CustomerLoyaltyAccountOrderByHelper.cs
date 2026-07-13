using System.Linq.Expressions;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Helpers;

public static class CustomerLoyaltyAccountOrderByHelper
{
    public static Expression<Func<CustomerLoyaltyAccount, object>> Build(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "accountnumber" => x => x.AccountNumber!,
            "currentbalance" => x => x.CurrentBalance,
            "averagecostperthousand" => x => x.AverageCostPerThousand!,
            "status" => x => x.Status,
            "loyaltyprogramname" => x => x.LoyaltyProgram.Name,
            "lastbalanceupdateutc" => x => x.LastBalanceUpdateUtc!,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc
        };
    }
}
