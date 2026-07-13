using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Pricing;

public sealed class UpdatePaymentFeeRuleItem
{
    public PaymentMethod PaymentMethod { get; init; }
    public int Installments { get; init; }
    public decimal FeePercentage { get; init; }
    public bool IsActive { get; init; } = true;
}
