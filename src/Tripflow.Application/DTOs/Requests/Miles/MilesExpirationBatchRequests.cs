using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Miles;

public sealed class MilesExpirationBatchFilterRequest : FilterRequest
{
    public MilesExpirationStatus? Status { get; set; }
    public DateOnly? ExpiresFrom { get; set; }
    public DateOnly? ExpiresTo { get; set; }
}

public sealed record CreateMilesExpirationBatchRequest(
    Guid CustomerLoyaltyAccountId,
    int Amount,
    DateOnly ExpiresAt,
    MilesExpirationStatus? Status);

public sealed record UpdateMilesExpirationBatchRequest(
    int Amount,
    DateOnly ExpiresAt);
