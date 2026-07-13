using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Quotes;

public sealed record QuoteItemResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteId,
    QuoteItemType Type,
    string Title,
    string? Description,
    decimal AgencyCost,
    decimal SaleAmount,
    decimal EstimatedProfitAmount,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
