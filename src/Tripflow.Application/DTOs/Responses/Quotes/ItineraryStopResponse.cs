namespace Tripflow.Application.DTOs.Responses.Quotes;

public sealed record ItineraryStopResponse(
    Guid Id,
    Guid TenantId,
    Guid ItineraryId,
    string Country,
    string City,
    int Nights,
    int Sequence,
    string? Notes);
