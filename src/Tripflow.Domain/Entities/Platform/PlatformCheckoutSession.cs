using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Entities.Platform;

public sealed class PlatformCheckoutSession : AuditableEntity
{
    private PlatformCheckoutSession() { }

    public PlatformCheckoutSession(
        Guid leadId,
        Guid subscriptionPlanId,
        decimal amount,
        string currency,
        PlatformCheckoutStatus status,
        DateTime? expiresAtUtc,
        string createdBy)
    {
        LeadId = leadId;
        SubscriptionPlanId = subscriptionPlanId;
        Amount = amount;
        Currency = currency;
        Status = status;
        ExpiresAtUtc = expiresAtUtc;
        SetCreated(createdBy);
    }

    public Guid LeadId { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    public string? ExternalCheckoutId { get; private set; }
    public string? CheckoutUrl { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;
    public PlatformCheckoutStatus Status { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public string? RawProviderResponse { get; private set; }

    public void SetProviderDetails(string externalCheckoutId, string checkoutUrl, string? rawProviderResponse, string updatedBy)
    {
        ExternalCheckoutId = externalCheckoutId;
        CheckoutUrl = checkoutUrl;
        RawProviderResponse = rawProviderResponse;
        Status = PlatformCheckoutStatus.Active;
        SetUpdated(updatedBy);
    }

    public void MarkAsPaid(DateTime? paidAtUtc, string updatedBy)
    {
        Status = PlatformCheckoutStatus.Paid;
        PaidAtUtc = paidAtUtc ?? DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void MarkAsFailed(string updatedBy)
    {
        Status = PlatformCheckoutStatus.Failed;
        SetUpdated(updatedBy);
    }

    public void MarkAsCancelled(string updatedBy)
    {
        Status = PlatformCheckoutStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    public void MarkAsExpired(string updatedBy)
    {
        Status = PlatformCheckoutStatus.Expired;
        SetUpdated(updatedBy);
    }
}
