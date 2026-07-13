namespace Tripflow.Application.DTOs.Requests.Pricing;

public sealed record UpdateQuotePricingOptionRequest(
    string Name,
    decimal AgencyCost,
    decimal? DesiredProfitAmount,
    decimal? DesiredProfitPercentage,
    decimal? PixDiscountPercentage,
    string? InternalNotes,
    bool RegeneratePaymentConditions);
