namespace Tripflow.Application.DTOs.Responses.Pricing;

public sealed record QuotePricingOptionResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteId,
    string Name,
    decimal AgencyCost,
    decimal? DesiredProfitAmount,
    decimal? DesiredProfitPercentage,
    decimal? PixDiscountPercentage,
    decimal? PixAmount,
    decimal? CreditCashAmount,
    bool Selected,
    string? InternalNotes,
    IEnumerable<QuotePaymentConditionResponse> PaymentConditions,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
