using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Miles;

public sealed class MilesTransactionFilterRequest : FilterRequest
{
    public MilesTransactionType? Type { get; set; }
    public DateTime? TransactionFromUtc { get; set; }
    public DateTime? TransactionToUtc { get; set; }
}

public sealed record CreateMilesTransactionRequest(
    Guid CustomerLoyaltyAccountId,
    MilesTransactionType Type,
    int Amount,
    decimal? CostPerThousand,
    string? Description,
    DateTime? TransactionDateUtc,
    DateOnly? ExpiresAt,
    bool CreateExpirationBatch);
