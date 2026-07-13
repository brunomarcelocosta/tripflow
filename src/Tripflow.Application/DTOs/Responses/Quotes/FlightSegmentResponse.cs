namespace Tripflow.Application.DTOs.Responses.Quotes;

public sealed record FlightSegmentResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteFlightItemId,
    string OriginAirport,
    string DestinationAirport,
    string? OriginCity,
    string? DestinationCity,
    DateTime? DepartureDateTimeUtc,
    DateTime? ArrivalDateTimeUtc,
    string? FlightNumber,
    string? AirlineName,
    int Sequence);
