using System.Linq.Expressions;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Helpers;

public static class MilesExpirationBatchOrderByHelper
{
    public static Expression<Func<MilesExpirationBatch, object>> Build(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "amount" => x => x.Amount,
            "remainingamount" => x => x.RemainingAmount,
            "expiresat" => x => x.ExpiresAt,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.ExpiresAt
        };
    }
}
