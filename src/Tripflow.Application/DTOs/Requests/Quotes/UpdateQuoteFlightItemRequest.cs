namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record UpdateQuoteFlightItemRequest(
    string? AirlineName,
    string? FareFamily,
    string? BaggageDescription,
    bool IncludedPersonalItem,
    bool IncludedCarryOn,
    decimal? CarryOnWeightKg,
    bool IncludedCheckedBag,
    decimal? CheckedBagWeightKg);
