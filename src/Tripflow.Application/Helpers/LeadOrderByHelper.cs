using System.Linq.Expressions;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Application.Helpers;

public static class LeadOrderByHelper
{
    public static Expression<Func<Lead, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "companyname" => x => x.CompanyName,
            "responsiblename" => x => x.ResponsibleName,
            "email" => x => x.Email,
            "convertedtotenant" => x => x.ConvertedToTenant,
            "updatedatutc" => x => x.UpdatedAtUtc ?? x.CreatedAtUtc,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc
        };
    }
}
