namespace Tripflow.Application.DTOs.Requests.Miles;

public sealed record CreateMilesQuoteOptionRequest(
    Guid? LoyaltyProgramId,
    string Name,
    int MilesAmount,
    decimal? BoardingFees,
    decimal? CostPerThousand,
    decimal? CashPrice,
    decimal? ServiceFee,
    string? Notes);
