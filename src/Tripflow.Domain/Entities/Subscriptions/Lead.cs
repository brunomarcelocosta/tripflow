using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Entities.Subscriptions;

public sealed class Lead : AuditableEntity
{
    private Lead() { }

    public Lead(string companyName, string responsibleName, string email, string? phone, string? planOfInterest, string? message, string createdBy, string? source = null)
    {
        CompanyName = companyName;
        ResponsibleName = responsibleName;
        Email = email;
        Phone = phone;
        PlanOfInterest = planOfInterest;
        Message = message;
        Source = source;
        PaymentStatus = LeadPaymentStatus.None;
        SetCreated(createdBy);
    }

    public string CompanyName { get; private set; } = default!;
    public string ResponsibleName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? PlanOfInterest { get; private set; }
    public string? Message { get; private set; }
    public Guid? SubscriptionPlanId { get; private set; }
    public Guid? PlatformCheckoutSessionId { get; private set; }
    public LeadPaymentStatus PaymentStatus { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public string? Source { get; private set; }
    public bool ConvertedToTenant { get; private set; }
    public Guid? ConvertedTenantId { get; private set; }

    public void AssignSubscriptionPlan(Guid subscriptionPlanId, string updatedBy)
    {
        SubscriptionPlanId = subscriptionPlanId;
        SetUpdated(updatedBy);
    }

    public void AssignCheckoutSession(Guid platformCheckoutSessionId, string updatedBy)
    {
        PlatformCheckoutSessionId = platformCheckoutSessionId;
        PaymentStatus = LeadPaymentStatus.Pending;
        SetUpdated(updatedBy);
    }

    public void MarkAsPaid(DateTime? paidAtUtc, string updatedBy)
    {
        PaymentStatus = LeadPaymentStatus.Paid;
        PaidAtUtc = paidAtUtc ?? DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void MarkAsConverted(Guid tenantId, string updatedBy)
    {
        ConvertedToTenant = true;
        ConvertedTenantId = tenantId;
        SetUpdated(updatedBy);
    }
}
