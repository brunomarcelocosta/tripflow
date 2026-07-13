using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Payments;

public sealed class PaymentLink : AuditableEntity, ITenantEntity
{
    private PaymentLink() { }

    public PaymentLink(Guid tenantId, Guid paymentId, string? externalLinkId, string url, DateTime? expiresAtUtc, PaymentLinkStatus status, string createdBy)
    {
        TenantId = tenantId;
        PaymentId = paymentId;
        ExternalLinkId = externalLinkId;
        Url = url;
        ExpiresAtUtc = expiresAtUtc;
        Status = status;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid PaymentId { get; private set; }
    public Payment Payment { get; private set; } = default!;

    public string? ExternalLinkId { get; private set; }
    public string Url { get; private set; } = default!;
    public DateTime? ExpiresAtUtc { get; private set; }
    public PaymentLinkStatus Status { get; private set; }

    public void Cancel(string updatedBy)
    {
        Status = PaymentLinkStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    public void MarkAsPaid(string updatedBy)
    {
        Status = PaymentLinkStatus.Paid;
        SetUpdated(updatedBy);
    }

    public void Expire(string updatedBy)
    {
        Status = PaymentLinkStatus.Expired;
        SetUpdated(updatedBy);
    }
}

