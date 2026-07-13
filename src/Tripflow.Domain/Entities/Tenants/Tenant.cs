using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Entities.Tenants;

public sealed class Tenant : AuditableEntity
{
    private Tenant() { }

    public Tenant(
        string legalName,
        string tradeName,
        string? documentNumber,
        string? email,
        string? phone,
        TenantStatus status,
        string createdBy)
    {
        LegalName = legalName;
        TradeName = tradeName;
        DocumentNumber = documentNumber;
        Email = email;
        Phone = phone;
        Status = status;
        SetCreated(createdBy);
    }

    public string LegalName { get; private set; } = default!;
    public string TradeName { get; private set; } = default!;
    public string? DocumentNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public TenantStatus Status { get; private set; }

    public TenantBranding? Branding { get; private set; }
    public TenantCommercialSettings? CommercialSettings { get; private set; }
    public TenantSubscription? CurrentSubscription { get; private set; }

    public List<UserProfile> Users = [];
    public List<Customer> Customers = [];
    public List<Quote> Quotes = [];

    public void Update(string legalName, string tradeName, string? documentNumber, string? email, string? phone, TenantStatus status, string updatedBy)
    {
        LegalName = legalName;
        TradeName = tradeName;
        DocumentNumber = documentNumber;
        Email = email;
        Phone = phone;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = TenantStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Suspend(string updatedBy)
    {
        Status = TenantStatus.Suspended;
        SetUpdated(updatedBy);
    }

    public void Cancel(string updatedBy)
    {
        Status = TenantStatus.Cancelled;
        SetUpdated(updatedBy);
    }
}
