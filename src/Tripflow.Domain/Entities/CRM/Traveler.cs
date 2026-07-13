using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.CRM;

public sealed class Traveler : AuditableEntity, ITenantEntity
{
    private Traveler() { }

    public Traveler(Guid tenantId, Guid customerId, string fullName, DateOnly? birthDate, string? nationality, string? documentNumber, string? passportNumber, DateOnly? passportExpirationDate, string? notes, string createdBy)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        FullName = fullName;
        BirthDate = birthDate;
        Nationality = nationality;
        DocumentNumber = documentNumber;
        PassportNumber = passportNumber;
        PassportExpirationDate = passportExpirationDate;
        Notes = notes;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;

    public string FullName { get; private set; } = default!;
    public DateOnly? BirthDate { get; private set; }
    public string? Nationality { get; private set; }
    public string? DocumentNumber { get; private set; }
    public string? PassportNumber { get; private set; }
    public DateOnly? PassportExpirationDate { get; private set; }
    public string? Notes { get; private set; }

    public void Update(
        string fullName,
        DateOnly? birthDate,
        string? nationality,
        string? documentNumber,
        string? passportNumber,
        DateOnly? passportExpirationDate,
        string? notes,
        string updatedBy)
    {
        FullName = fullName;
        BirthDate = birthDate;
        Nationality = nationality;
        DocumentNumber = documentNumber;
        PassportNumber = passportNumber;
        PassportExpirationDate = passportExpirationDate;
        Notes = notes;
        SetUpdated(updatedBy);
    }

    public bool IsPassportExpired()
    {
        if (!PassportExpirationDate.HasValue)
            return false;

        return PassportExpirationDate.Value < DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public bool IsPassportExpiringSoon(int months = 6)
    {
        if (!PassportExpirationDate.HasValue)
            return false;

        if (IsPassportExpired())
            return false;

        var threshold = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(months);
        return PassportExpirationDate.Value <= threshold;
    }
}
