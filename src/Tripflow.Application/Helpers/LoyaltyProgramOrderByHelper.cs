using System.Linq.Expressions;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Application.Helpers;

public static class LoyaltyProgramOrderByHelper
{
    public static Expression<Func<LoyaltyProgram, object>> Build(string? sortBy)
    {
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "name" => x => x.Name,
            "country" => x => x.Country!,
            "airlinename" => x => x.AirlineName!,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc
        };
    }
}
