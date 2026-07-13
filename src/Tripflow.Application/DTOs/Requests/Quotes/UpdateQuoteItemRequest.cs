using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record UpdateQuoteItemRequest(
    QuoteItemType Type,
    string Title,
    string? Description,
    decimal AgencyCost,
    decimal SaleAmount,
    string? Notes);
