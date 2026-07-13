namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record UpdateItineraryStopRequest(
    string Country,
    string City,
    int Nights,
    int Sequence,
    string? Notes);
