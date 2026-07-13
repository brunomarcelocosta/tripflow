using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record UpdateQuoteRequest(
    Guid? CustomerId,
    string Title,
    QuoteType Type,
    string? Origin,
    string? Destination,
    DateOnly? DepartureDate,
    DateOnly? ReturnDate,
    int Adults,
    int Children,
    int Infants,
    string? Notes,
    DateTime? ExpiresAtUtc);
