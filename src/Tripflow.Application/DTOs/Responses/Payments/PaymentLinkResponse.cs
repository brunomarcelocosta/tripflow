using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Payments;

public sealed record PaymentLinkResponse(
    Guid Id,
    Guid TenantId,
    Guid PaymentId,
    string? ExternalLinkId,
    string Url,
    DateTime? ExpiresAtUtc,
    PaymentLinkStatus Status);
