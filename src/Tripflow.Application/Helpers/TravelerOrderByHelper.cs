using System.Linq.Expressions;
using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Application.Helpers;

public static class TravelerOrderByHelper
{
    public static Expression<Func<Traveler, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "fullname" => x => x.FullName,
            "nationality" => x => x.Nationality!,
            "documentnumber" => x => x.DocumentNumber!,
            "passportnumber" => x => x.PassportNumber!,
            "passportexpirationdate" => x => x.PassportExpirationDate!,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }
}
