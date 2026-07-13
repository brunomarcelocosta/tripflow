using System.Linq.Expressions;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Application.Helpers;

public static class FinancialTransactionOrderByHelper
{
    public static Expression<Func<FinancialTransaction, object>>? Build(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "type" => x => x.Type,
            "grossamount" => x => x.GrossAmount,
            "transactiondateutc" => x => x.TransactionDateUtc,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.TransactionDateUtc
        };
    }
}
