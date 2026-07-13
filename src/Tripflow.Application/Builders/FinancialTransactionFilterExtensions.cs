using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Application.Builders;

public static class FinancialTransactionFilterExtensions
{
    public static Expression<Func<FinancialTransaction, bool>> ToExpression(this FinancialTransactionFilterRequest request)
    {
        Expression<Func<FinancialTransaction, bool>> filter = x => true;

        if (request.PaymentId.HasValue)
        {
            var paymentId = request.PaymentId.Value;
            filter = filter.And(x => x.PaymentId == paymentId);
        }

        if (request.QuoteId.HasValue)
        {
            var quoteId = request.QuoteId.Value;
            filter = filter.And(x => x.QuoteId == quoteId);
        }

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
