using System.Linq.Expressions;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Helpers;

public static class MilesTransactionOrderByHelper
{
    public static Expression<Func<MilesTransaction, object>> Build(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "type" => x => x.Type,
            "amount" => x => x.Amount,
            "costperthousand" => x => x.CostPerThousand!,
            "totalcost" => x => x.TotalCost!,
            "transactiondateutc" => x => x.TransactionDateUtc,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.TransactionDateUtc
        };
    }
}
