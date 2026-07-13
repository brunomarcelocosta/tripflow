using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Subscriptions;

public sealed class TenantSubscription : AuditableEntity, ITenantEntity
{
    private TenantSubscription() { }

    public TenantSubscription(Guid tenantId, Guid subscriptionPlanId, TenantSubscriptionStatus status, DateTime startedAtUtc, DateTime? expiresAtUtc, DateTime? trialEndsAtUtc, string createdBy)
    {
        TenantId = tenantId;
        SubscriptionPlanId = subscriptionPlanId;
        Status = status;
        StartedAtUtc = startedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        TrialEndsAtUtc = trialEndsAtUtc;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid SubscriptionPlanId { get; private set; }
    public SubscriptionPlan SubscriptionPlan { get; private set; } = default!;

    public TenantSubscriptionStatus Status { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public DateTime? TrialEndsAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    public void UpdatePlan(Guid subscriptionPlanId, TenantSubscriptionStatus status, DateTime startedAtUtc, DateTime? expiresAtUtc, DateTime? trialEndsAtUtc, string updatedBy)
    {
        SubscriptionPlanId = subscriptionPlanId;
        Status = status;
        StartedAtUtc = startedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        TrialEndsAtUtc = trialEndsAtUtc;
        CancelledAtUtc = null;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = TenantSubscriptionStatus.Active;
        CancelledAtUtc = null;
        SetUpdated(updatedBy);
    }

    public void Suspend(string updatedBy)
    {
        Status = TenantSubscriptionStatus.Suspended;
        SetUpdated(updatedBy);
    }

    public void Cancel(string updatedBy)
    {
        Status = TenantSubscriptionStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void MarkPastDue(string updatedBy)
    {
        Status = TenantSubscriptionStatus.PastDue;
        SetUpdated(updatedBy);
    }

    public bool IsUsable()
    {
        if (Status is TenantSubscriptionStatus.Cancelled or TenantSubscriptionStatus.Suspended)
            return false;

        if (Status == TenantSubscriptionStatus.Trial && TrialEndsAtUtc.HasValue && DateTime.UtcNow > TrialEndsAtUtc.Value)
            return false;

        if (ExpiresAtUtc.HasValue && DateTime.UtcNow > ExpiresAtUtc.Value)
            return false;

        return Status is TenantSubscriptionStatus.Active or TenantSubscriptionStatus.Trial or TenantSubscriptionStatus.PastDue;
    }
}
