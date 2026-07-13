using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Payments;

public sealed record UpdateTenantPaymentProviderRequest(
    string? DisplayName,
    string? ApiKey,
    string? Secret,
    string? WebhookSecret,
    bool IsDefault,
    PaymentProviderStatus Status);
