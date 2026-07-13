using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Application.Builders;

public static class PaymentFilterExtensions
{
    public static Expression<Func<Payment, bool>> ToExpression(this PaymentFilterRequest request)
    {
        Expression<Func<Payment, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            filter = filter.And(x =>
                (x.ExternalPaymentId != null && x.ExternalPaymentId.ToLower().Contains(search)));
        }

        if (request.ProposalId.HasValue)
        {
            var proposalId = request.ProposalId.Value;
            filter = filter.And(x => x.ProposalId == proposalId);
        }

        if (request.QuoteId.HasValue)
        {
            var quoteId = request.QuoteId.Value;
            filter = filter.And(x => x.QuoteId == quoteId);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        if (request.PaymentMethod.HasValue)
        {
            var paymentMethod = request.PaymentMethod.Value;
            filter = filter.And(x => x.PaymentMethod == paymentMethod);
        }

        if (request.DueDateFrom.HasValue)
        {
            var from = request.DueDateFrom.Value;
            filter = filter.And(x => x.DueDate.HasValue && x.DueDate.Value >= from);
        }

        if (request.DueDateTo.HasValue)
        {
            var to = request.DueDateTo.Value;
            filter = filter.And(x => x.DueDate.HasValue && x.DueDate.Value <= to);
        }

        if (request.PaidFromUtc.HasValue)
        {
            var from = request.PaidFromUtc.Value;
            filter = filter.And(x => x.PaidAtUtc.HasValue && x.PaidAtUtc.Value >= from);
        }

        if (request.PaidToUtc.HasValue)
        {
            var to = request.PaidToUtc.Value;
            filter = filter.And(x => x.PaidAtUtc.HasValue && x.PaidAtUtc.Value <= to);
        }

        return filter;
    }
}
