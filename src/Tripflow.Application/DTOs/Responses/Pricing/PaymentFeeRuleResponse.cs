using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Pricing;

public sealed class PaymentFeeRuleResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public int Installments { get; init; }
    public decimal FeePercentage { get; init; }
    public bool IsActive { get; init; }
}
