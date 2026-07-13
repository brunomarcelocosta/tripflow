using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.CRM;

public sealed class CustomerPreference : AuditableEntity, ITenantEntity
{
    private CustomerPreference() { }

    public CustomerPreference(Guid tenantId, Guid customerId, string createdBy)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;

    public string? PreferredAirlines { get; private set; }
    public string? PreferredHotelCategories { get; private set; }
    public string? SeatPreferences { get; private set; }
    public string? MealRestrictions { get; private set; }
    public string? TravelPreferences { get; private set; }
    public string? GeneralNotes { get; private set; }

    public void Update(
        string? preferredAirlines,
        string? preferredHotelCategories,
        string? seatPreferences,
        string? mealRestrictions,
        string? travelPreferences,
        string? generalNotes,
        string updatedBy)
    {
        PreferredAirlines = preferredAirlines;
        PreferredHotelCategories = preferredHotelCategories;
        SeatPreferences = seatPreferences;
        MealRestrictions = mealRestrictions;
        TravelPreferences = travelPreferences;
        GeneralNotes = generalNotes;
        SetUpdated(updatedBy);
    }
}
