using System.Linq.Expressions;
using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Application.Helpers;

public static class CustomerOrderByHelper
{
    public static Expression<Func<Customer, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "fullname" => x => x.FullName,
            "email" => x => x.Email!,
            "phone" => x => x.Phone!,
            "documentnumber" => x => x.DocumentNumber!,
            "type" => x => x.Type,
            "status" => x => x.Status,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }
}
