namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record CreateItineraryStopRequest(
    string Country,
    string City,
    int Nights,
    int Sequence,
    string? Notes);
