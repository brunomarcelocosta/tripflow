using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Builders;

public static class MilesTransactionFilterExtensions
{
    public static Expression<Func<MilesTransaction, bool>> ToExpression(this MilesTransactionFilterRequest request, Guid accountId)
    {
        Expression<Func<MilesTransaction, bool>> filter = x => x.CustomerLoyaltyAccountId == accountId;

        if (request.Type.HasValue)
        {
            var type = request.Type.Value;
            filter = filter.And(x => x.Type == type);
        }

        if (request.TransactionFromUtc.HasValue)
        {
            var from = request.TransactionFromUtc.Value;
            filter = filter.And(x => x.TransactionDateUtc >= from);
        }

        if (request.TransactionToUtc.HasValue)
        {
            var to = request.TransactionToUtc.Value;
            filter = filter.And(x => x.TransactionDateUtc <= to);
        }

        return filter;
    }
}
