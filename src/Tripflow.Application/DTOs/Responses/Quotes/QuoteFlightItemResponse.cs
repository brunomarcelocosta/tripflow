namespace Tripflow.Application.DTOs.Responses.Quotes;

public sealed record QuoteFlightItemResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteId,
    string? AirlineName,
    string? FareFamily,
    string? BaggageDescription,
    bool IncludedPersonalItem,
    bool IncludedCarryOn,
    decimal? CarryOnWeightKg,
    bool IncludedCheckedBag,
    decimal? CheckedBagWeightKg,
    IEnumerable<FlightSegmentResponse> Segments);
