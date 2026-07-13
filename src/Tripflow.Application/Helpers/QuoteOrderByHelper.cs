using System.Linq.Expressions;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Application.Helpers;

public static class QuoteOrderByHelper
{
    public static Expression<Func<Quote, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "quotenumber" => x => x.QuoteNumber,
            "title" => x => x.Title,
            "type" => x => x.Type,
            "status" => x => x.Status,
            "origin" => x => x.Origin!,
            "destination" => x => x.Destination!,
            "departuredate" => x => x.DepartureDate!,
            "returndate" => x => x.ReturnDate!,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }
}
