namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record UpdateItineraryRequest(
    string Name,
    int? TotalDays,
    int? TotalNights);
