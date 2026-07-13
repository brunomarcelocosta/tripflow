using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Miles;

public sealed record MilesExpirationBatchResponse(
    Guid Id,
    Guid TenantId,
    Guid CustomerLoyaltyAccountId,
    int Amount,
    int RemainingAmount,
    DateOnly ExpiresAt,
    MilesExpirationStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc)
{
    public bool IsExpired => ExpiresAt < DateOnly.FromDateTime(DateTime.UtcNow) || Status == MilesExpirationStatus.Expired;
    public int DaysToExpire => ExpiresAt.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
}
