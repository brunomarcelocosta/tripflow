using System.Linq.Expressions;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Application.Helpers;

public static class PaymentOrderByHelper
{
    public static Expression<Func<Payment, object>> Build(string? sortBy)
    {
        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "status" => x => x.Status,
            "paymentmethod" => x => x.PaymentMethod,
            "grossamount" => x => x.GrossAmount,
            "feeamount" => x => x.FeeAmount!,
            "netamount" => x => x.NetAmount!,
            "installments" => x => x.Installments!,
            "duedate" => x => x.DueDate!,
            "paidatutc" => x => x.PaidAtUtc!,
            "updatedatutc" => x => x.UpdatedAtUtc!,
            "createdatutc" => x => x.CreatedAtUtc,
            _ => x => x.CreatedAtUtc,
        };
    }
}
