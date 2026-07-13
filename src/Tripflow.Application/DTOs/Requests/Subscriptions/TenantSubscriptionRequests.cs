using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Subscriptions;

public sealed class UpdateTenantSubscriptionRequest
{
    public Guid SubscriptionPlanId { get; init; }
    public TenantSubscriptionStatus Status { get; init; } = TenantSubscriptionStatus.Active;
    public DateTime StartedAtUtc { get; init; }
    public DateTime? ExpiresAtUtc { get; init; }
    public DateTime? TrialEndsAtUtc { get; init; }
}
