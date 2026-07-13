using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Application.Builders;

public static class TenantFilterBuilder
{
    public static Expression<Func<Tenant, bool>> ToExpression(this TenantFilterRequest request)
    {
        var parameter = Expression.Parameter(typeof(Tenant), "x");
        Expression<Func<Tenant, bool>> expr = Expression.Lambda<Func<Tenant, bool>>(Expression.Constant(true), parameter);

        if (!string.IsNullOrWhiteSpace(request.LegalName))
        {
            var legal = request.LegalName.Trim().ToUpperInvariant();
            expr = expr.And(x => x.LegalName == legal);
        }

        if (!string.IsNullOrWhiteSpace(request.TradeName))
        {
            var trade = request.TradeName.Trim().ToUpperInvariant();
            expr = expr.And(x => x.TradeName == trade);
        }

        if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            var doc = request.DocumentNumber.Trim().ToUpperInvariant();
            expr = expr.And(x => x.DocumentNumber == doc);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToUpperInvariant();
            expr = expr.And(x => x.Email == email);
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            expr = expr.And(x => x.Status == status);
        }

        return expr;
    }
}
