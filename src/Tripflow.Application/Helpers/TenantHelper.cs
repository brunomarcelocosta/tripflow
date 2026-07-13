using System.Linq.Expressions;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Application.Helpers;

public static class TenantOrderByHelper
{
    public static Expression<Func<Tenant, object>> Build(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "legalname" => x => x.LegalName,
            "tradename" => x => x.TradeName,
            "documentnumber" => x => x.DocumentNumber!,
            "email" => x => x.Email!,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc
        };
    }
}