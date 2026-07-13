using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Proposals;

public sealed record ProposalResponse(
    Guid Id,
    Guid TenantId,
    Guid QuoteId,
    Guid? QuotePricingOptionId,
    string ProposalNumber,
    ProposalStatus Status,
    string? PublicToken,
    string? PublicUrl,
    string? PdfUrl,
    DateTime? ViewedAtUtc,
    DateTime? ApprovedAtUtc,
    DateTime? RejectedAtUtc,
    DateTime? ExpiresAtUtc,
    int VersionsCount,
    int EventsCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
