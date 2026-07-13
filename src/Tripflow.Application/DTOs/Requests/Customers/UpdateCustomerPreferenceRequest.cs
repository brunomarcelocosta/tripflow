namespace Tripflow.Application.DTOs.Requests.Customers;

public sealed record UpdateCustomerPreferenceRequest(
    string? PreferredAirlines,
    string? PreferredHotelCategories,
    string? SeatPreferences,
    string? MealRestrictions,
    string? TravelPreferences,
    string? GeneralNotes);
