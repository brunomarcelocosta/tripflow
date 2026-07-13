using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Miles;

public sealed record MilesTransactionResponse(
    Guid Id,
    Guid TenantId,
    Guid CustomerLoyaltyAccountId,
    MilesTransactionType Type,
    int Amount,
    decimal? CostPerThousand,
    decimal? TotalCost,
    string? Description,
    DateTime TransactionDateUtc,
    DateTime CreatedAtUtc);
