using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Application.Builders;

public static class ProposalFilterExtensions
{
    public static Expression<Func<Proposal, bool>> ToExpression(this ProposalFilterRequest request)
    {
        Expression<Func<Proposal, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            filter = filter.And(x => x.ProposalNumber.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.ProposalNumber))
        {
            var number = request.ProposalNumber.Trim().ToLower();
            filter = filter.And(x => x.ProposalNumber.ToLower().Contains(number));
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

        if (request.CreatedFromUtc.HasValue)
        {
            var from = request.CreatedFromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.CreatedToUtc.HasValue)
        {
            var to = request.CreatedToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        if (request.ExpiresFromUtc.HasValue)
        {
            var from = request.ExpiresFromUtc.Value;
            filter = filter.And(x => x.ExpiresAtUtc.HasValue && x.ExpiresAtUtc.Value >= from);
        }

        if (request.ExpiresToUtc.HasValue)
        {
            var to = request.ExpiresToUtc.Value;
            filter = filter.And(x => x.ExpiresAtUtc.HasValue && x.ExpiresAtUtc.Value <= to);
        }

        return filter;
    }
}
