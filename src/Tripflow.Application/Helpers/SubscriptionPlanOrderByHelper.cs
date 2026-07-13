using System.Linq.Expressions;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Application.Helpers;

public static class SubscriptionPlanOrderByHelper
{
    public static Expression<Func<SubscriptionPlan, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "name" => x => x.Name,
            "monthlyprice" => x => x.MonthlyPrice ?? 0m,
            "annualprice" => x => x.AnnualPrice ?? 0m,
            "maxusers" => x => x.MaxUsers ?? 0,
            "maxquotespermonth" => x => x.MaxQuotesPerMonth ?? 0,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc ?? x.CreatedAtUtc,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc
        };
    }
}
