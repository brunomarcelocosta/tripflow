using System.Linq.Expressions;
using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Application.Helpers;

public static class ProposalOrderByHelper
{
    public static Expression<Func<Proposal, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "proposalnumber" => x => x.ProposalNumber,
            "quoteid" => x => x.QuoteId,
            "status" => x => x.Status,
            "expiresatutc" => x => x.ExpiresAtUtc!,
            "viewedatutc" => x => x.ViewedAtUtc!,
            "approvedatutc" => x => x.ApprovedAtUtc!,
            "rejectedatutc" => x => x.RejectedAtUtc!,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }
}
