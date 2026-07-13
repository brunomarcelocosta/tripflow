using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Payments;

public sealed record PaymentResponse(
    Guid Id,
    Guid TenantId,
    Guid? QuoteId,
    Guid? ProposalId,
    Guid? TenantPaymentProviderId,
    string? ExternalPaymentId,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    int? Installments,
    decimal GrossAmount,
    decimal? FeeAmount,
    decimal? NetAmount,
    DateOnly? DueDate,
    DateTime? PaidAtUtc,
    IEnumerable<PaymentLinkResponse> Links,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
