using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Application.Builders;

public static class LeadFilterExtensions
{
    public static Expression<Func<Lead, bool>> ToExpression(this LeadFilterRequest request)
    {
        Expression<Func<Lead, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.CompanyName.ToLower().Contains(search) ||
                x.ResponsibleName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search) ||
                (x.Phone != null && x.Phone.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var companyName = request.Name.Trim().ToLowerInvariant();
            filter = filter.And(x => x.CompanyName.ToLower().Contains(companyName));
        }

        if (!string.IsNullOrWhiteSpace(request.CompanyName))
        {
            var companyName = request.CompanyName.Trim().ToLowerInvariant();
            filter = filter.And(x => x.CompanyName.ToLower().Contains(companyName));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            filter = filter.And(x => x.Email.ToLower().Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(request.PlanOfInterest))
        {
            var plan = request.PlanOfInterest.Trim().ToLowerInvariant();
            filter = filter.And(x => x.PlanOfInterest != null && x.PlanOfInterest.ToLower().Contains(plan));
        }

        if (request.ConvertedToTenant.HasValue)
        {
            var converted = request.ConvertedToTenant.Value;
            filter = filter.And(x => x.ConvertedToTenant == converted);
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

        return filter;
    }
}
