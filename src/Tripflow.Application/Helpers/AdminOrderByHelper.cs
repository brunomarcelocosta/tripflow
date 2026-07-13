using System.Linq.Expressions;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Application.Helpers;

public static class AdminOrderByHelper
{
    public static Expression<Func<Tenant, object>> BuildTenant(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "legalname" => x => x.LegalName,
            "tradename" => x => x.TradeName,
            "documentnumber" => x => x.DocumentNumber!,
            "email" => x => x.Email!,
            "phone" => x => x.Phone!,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }

    public static Expression<Func<UserProfile, object>> BuildUser(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "fullname" => x => x.FullName,
            "email" => x => x.Email,
            "phone" => x => x.Phone!,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }
}
