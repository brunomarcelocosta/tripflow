using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Payments;

public sealed record TenantPaymentProviderResponse(
    Guid Id,
    Guid TenantId,
    Guid PaymentProviderId,
    string PaymentProviderCode,
    string PaymentProviderName,
    string? DisplayName,
    bool HasApiKey,
    bool HasSecret,
    bool HasWebhookSecret,
    bool IsDefault,
    PaymentProviderStatus Status);
