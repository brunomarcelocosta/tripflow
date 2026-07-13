namespace Tripflow.Application.DTOs.Responses.Miles;

public sealed record MilesQuoteOptionResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteId,
    Guid? LoyaltyProgramId,
    string? LoyaltyProgramName,
    string Name,
    int MilesAmount,
    decimal? BoardingFees,
    decimal? CostPerThousand,
    decimal? EquivalentCost,
    decimal? CashPrice,
    decimal? EstimatedSavings,
    decimal? ServiceFee,
    decimal? TotalAmount,
    bool Selected,
    string? Notes);
