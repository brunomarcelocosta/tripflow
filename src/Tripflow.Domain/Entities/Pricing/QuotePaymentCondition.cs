using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Pricing;

public sealed class QuotePaymentCondition : AuditableEntity, ITenantEntity
{
    private QuotePaymentCondition() { }

    public QuotePaymentCondition(Guid tenantId, Guid quotePricingOptionId, PaymentMethod paymentMethod, int installments, decimal? feePercentage, decimal grossAmount, decimal? installmentAmount, decimal? estimatedFeeAmount, decimal? estimatedNetAmount, decimal? estimatedProfitAmount, string createdBy)
    {
        TenantId = tenantId;
        QuotePricingOptionId = quotePricingOptionId;
        PaymentMethod = paymentMethod;
        Installments = installments;
        FeePercentage = feePercentage;
        GrossAmount = grossAmount;
        InstallmentAmount = installmentAmount;
        EstimatedFeeAmount = estimatedFeeAmount;
        EstimatedNetAmount = estimatedNetAmount;
        EstimatedProfitAmount = estimatedProfitAmount;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuotePricingOptionId { get; private set; }
    public QuotePricingOption QuotePricingOption { get; private set; } = default!;

    public PaymentMethod PaymentMethod { get; private set; }
    public int Installments { get; private set; }
    public decimal? FeePercentage { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal? InstallmentAmount { get; private set; }
    public decimal? EstimatedFeeAmount { get; private set; }
    public decimal? EstimatedNetAmount { get; private set; }
    public decimal? EstimatedProfitAmount { get; private set; }
}

