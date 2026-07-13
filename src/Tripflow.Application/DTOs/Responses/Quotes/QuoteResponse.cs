using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Quotes;

public sealed record QuoteResponse(
    Guid Id,
    Guid TenantId,
    Guid? CustomerId,
    string? CustomerName,
    string QuoteNumber,
    string Title,
    QuoteType Type,
    QuoteStatus Status,
    string? Origin,
    string? Destination,
    DateOnly? DepartureDate,
    DateOnly? ReturnDate,
    int Adults,
    int Children,
    int Infants,
    string? Notes,
    DateTime? ExpiresAtUtc,
    int ItemsCount,
    int FlightItemsCount,
    bool HasItinerary,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
