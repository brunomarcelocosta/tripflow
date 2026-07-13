using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Application.Builders;

public static class TravelerFilterExtensions
{
    public static Expression<Func<Traveler, bool>> ToExpression(this TravelerFilterRequest request, Guid? customerId = null)
    {
        Expression<Func<Traveler, bool>> filter = x => true;

        if (customerId.HasValue)
        {
            var cid = customerId.Value;
            filter = filter.And(x => x.CustomerId == cid);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            filter = filter.And(x =>
                (x.FullName != null && x.FullName.ToLower().Contains(search)) ||
                (x.DocumentNumber != null && x.DocumentNumber.ToLower().Contains(search)) ||
                (x.PassportNumber != null && x.PassportNumber.ToLower().Contains(search)) ||
                (x.Nationality != null && x.Nationality.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLower();
            filter = filter.And(x => x.FullName != null && x.FullName.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var fullName = request.FullName.Trim().ToLower();
            filter = filter.And(x => x.FullName != null && x.FullName.ToLower().Contains(fullName));
        }

        if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            var doc = request.DocumentNumber.Trim();
            filter = filter.And(x => x.DocumentNumber == doc);
        }

        if (!string.IsNullOrWhiteSpace(request.PassportNumber))
        {
            var passport = request.PassportNumber.Trim();
            filter = filter.And(x => x.PassportNumber == passport);
        }

        if (!string.IsNullOrWhiteSpace(request.Nationality))
        {
            var nationality = request.Nationality.Trim().ToLower();
            filter = filter.And(x => x.Nationality != null && x.Nationality.ToLower() == nationality);
        }

        if (request.PassportExpiringBefore.HasValue)
        {
            var date = request.PassportExpiringBefore.Value;
            filter = filter.And(x =>
                x.PassportExpirationDate.HasValue
                && x.PassportExpirationDate.Value <= date);
        }

        return filter;
    }
}
