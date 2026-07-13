namespace Tripflow.Application.DTOs.Requests.Payments;

public sealed record CreateTenantPaymentProviderRequest(
    Guid PaymentProviderId,
    string? DisplayName,
    string? ApiKey,
    string? Secret,
    string? WebhookSecret,
    bool IsDefault);
