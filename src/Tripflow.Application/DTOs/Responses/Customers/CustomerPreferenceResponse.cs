namespace Tripflow.Application.DTOs.Responses.Customers;

public sealed record CustomerPreferenceResponse(
    Guid Id,
    Guid TenantId,
    Guid CustomerId,
    string? PreferredAirlines,
    string? PreferredHotelCategories,
    string? SeatPreferences,
    string? MealRestrictions,
    string? TravelPreferences,
    string? GeneralNotes,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
