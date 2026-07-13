using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Builders;

public static class CustomerLoyaltyAccountFilterExtensions
{
    public static Expression<Func<CustomerLoyaltyAccount, bool>> ToExpression(this CustomerLoyaltyAccountFilterRequest request, Guid customerId)
    {
        Expression<Func<CustomerLoyaltyAccount, bool>> filter = x => x.CustomerId == customerId;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                (x.AccountNumber != null && x.AccountNumber.ToLower().Contains(search)) ||
                x.LoyaltyProgram.Name.ToLower().Contains(search));
        }

        if (request.LoyaltyProgramId.HasValue)
        {
            var loyaltyProgramId = request.LoyaltyProgramId.Value;
            filter = filter.And(x => x.LoyaltyProgramId == loyaltyProgramId);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.AccountNumber))
        {
            var accountNumber = request.AccountNumber.Trim().ToLowerInvariant();
            filter = filter.And(x => x.AccountNumber != null && x.AccountNumber.ToLower().Contains(accountNumber));
        }

        return filter;
    }
}
