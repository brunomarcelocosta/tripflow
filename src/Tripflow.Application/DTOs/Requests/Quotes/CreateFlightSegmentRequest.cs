namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record CreateFlightSegmentRequest(
    string OriginAirport,
    string DestinationAirport,
    string? OriginCity,
    string? DestinationCity,
    DateTime? DepartureDateTimeUtc,
    DateTime? ArrivalDateTimeUtc,
    string? FlightNumber,
    string? AirlineName,
    int Sequence);
