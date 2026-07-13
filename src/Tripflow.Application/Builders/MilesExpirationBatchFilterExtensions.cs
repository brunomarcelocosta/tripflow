using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Builders;

public static class MilesExpirationBatchFilterExtensions
{
    public static Expression<Func<MilesExpirationBatch, bool>> ToExpression(this MilesExpirationBatchFilterRequest request, Guid accountId)
    {
        Expression<Func<MilesExpirationBatch, bool>> filter = x => x.CustomerLoyaltyAccountId == accountId;

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        if (request.ExpiresFrom.HasValue)
        {
            var from = request.ExpiresFrom.Value;
            filter = filter.And(x => x.ExpiresAt >= from);
        }

        if (request.ExpiresTo.HasValue)
        {
            var to = request.ExpiresTo.Value;
            filter = filter.And(x => x.ExpiresAt <= to);
        }

        return filter;
    }
}
