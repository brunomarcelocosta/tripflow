using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.CRM;

public sealed class Customer : AuditableEntity, ITenantEntity
{
    private Customer() { }

    public Customer(Guid tenantId, CustomerType type, string fullName, string? documentNumber, string? email, string? phone, DateOnly? birthDate, string? notes, string createdBy)
    {
        TenantId = tenantId;
        Type = type;
        FullName = fullName;
        DocumentNumber = documentNumber;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        Notes = notes;
        Status = CustomerStatus.Active;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public CustomerType Type { get; private set; }
    public string FullName { get; private set; } = default!;
    public string? DocumentNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Notes { get; private set; }
    public CustomerStatus Status { get; private set; }

    public CustomerPreference? Preference { get; private set; }
    public List<Traveler> Travelers = [];
    public List<CustomerLoyaltyAccount> LoyaltyAccounts = [];
    public List<Quote> Quotes = [];

    public void Update(
        CustomerType type,
        string fullName,
        string? documentNumber,
        string? email,
        string? phone,
        DateOnly? birthDate,
        string? notes,
        CustomerStatus status,
        string updatedBy)
    {
        Type = type;
        FullName = fullName;
        DocumentNumber = documentNumber;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        Notes = notes;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = CustomerStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Inactivate(string updatedBy)
    {
        Status = CustomerStatus.Inactive;
        SetUpdated(updatedBy);
    }

    public void Block(string updatedBy)
    {
        Status = CustomerStatus.Blocked;
        SetUpdated(updatedBy);
    }

    public bool CanReceiveQuotes() => Status == CustomerStatus.Active;
}
