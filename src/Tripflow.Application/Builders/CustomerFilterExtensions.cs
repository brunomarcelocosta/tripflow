using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Application.Builders;

public static class CustomerFilterExtensions
{
    public static Expression<Func<Customer, bool>> ToExpression(this CustomerFilterRequest request)
    {
        Expression<Func<Customer, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            filter = filter.And(x =>
                (x.FullName != null && x.FullName.ToLower().Contains(search)) ||
                (x.Email != null && x.Email.ToLower().Contains(search)) ||
                (x.Phone != null && x.Phone.ToLower().Contains(search)) ||
                (x.DocumentNumber != null && x.DocumentNumber.ToLower().Contains(search)));
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

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLower();
            filter = filter.And(x => x.Email != null && x.Email.ToLower() == email);
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phone = request.Phone.Trim();
            filter = filter.And(x => x.Phone != null && x.Phone.Contains(phone));
        }

        if (request.Type.HasValue)
        {
            var type = request.Type.Value;
            filter = filter.And(x => x.Type == type);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }
}
