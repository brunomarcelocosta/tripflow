using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Application.Builders;

public static class SubscriptionPlanFilterExtensions
{
    public static Expression<Func<SubscriptionPlan, bool>> ToExpression(this SubscriptionPlanFilterRequest request)
    {
        Expression<Func<SubscriptionPlan, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.Name.ToLower().Contains(search) ||
                (x.Description != null && x.Description.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLowerInvariant();
            filter = filter.And(x => x.Name.ToLower().Contains(name));
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }
}
