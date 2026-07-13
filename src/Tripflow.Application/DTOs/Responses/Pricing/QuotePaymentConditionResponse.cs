using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Pricing;

public sealed record QuotePaymentConditionResponse(
    Guid Id,
    Guid TenantId,
    Guid QuotePricingOptionId,
    PaymentMethod PaymentMethod,
    int Installments,
    decimal? FeePercentage,
    decimal GrossAmount,
    decimal? InstallmentAmount,
    decimal? EstimatedFeeAmount,
    decimal? EstimatedNetAmount,
    decimal? EstimatedProfitAmount);
