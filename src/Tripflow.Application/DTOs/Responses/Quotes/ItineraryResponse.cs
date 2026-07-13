namespace Tripflow.Application.DTOs.Responses.Quotes;

public sealed record ItineraryResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteId,
    string Name,
    int? TotalDays,
    int? TotalNights,
    IEnumerable<ItineraryStopResponse> Stops);
