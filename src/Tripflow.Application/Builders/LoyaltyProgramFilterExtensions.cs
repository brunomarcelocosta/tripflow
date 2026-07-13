using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Builders;

public static class LoyaltyProgramFilterExtensions
{
    public static Expression<Func<LoyaltyProgram, bool>> ToExpression(this LoyaltyProgramFilterRequest request)
    {
        Expression<Func<LoyaltyProgram, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.Name.ToLower().Contains(search) ||
                (x.Country != null && x.Country.ToLower().Contains(search)) ||
                (x.AirlineName != null && x.AirlineName.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLowerInvariant();
            filter = filter.And(x => x.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var country = request.Country.Trim().ToLowerInvariant();
            filter = filter.And(x => x.Country != null && x.Country.ToLower().Contains(country));
        }

        if (!string.IsNullOrWhiteSpace(request.AirlineName))
        {
            var airlineName = request.AirlineName.Trim().ToLowerInvariant();
            filter = filter.And(x => x.AirlineName != null && x.AirlineName.ToLower().Contains(airlineName));
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }
}
