namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record UpdateFlightSegmentRequest(
    string OriginAirport,
    string DestinationAirport,
    string? OriginCity,
    string? DestinationCity,
    DateTime? DepartureDateTimeUtc,
    DateTime? ArrivalDateTimeUtc,
    string? FlightNumber,
    string? AirlineName,
    int Sequence);
