namespace Tripflow.Application.Services.Payments.InfinitePay;

public sealed record CreateInfinitePayPlatformCheckoutCommand(
    string CustomerName,
    string CustomerEmail,
    string? CustomerPhone,
    string PlanName,
    Guid SubscriptionPlanId,
    decimal Amount,
    string Currency,
    string SuccessUrl,
    string CancelUrl,
    string? ExternalReference);

public sealed record CreateInfinitePayTenantCheckoutCommand(
    Guid TenantId,
    string CustomerName,
    string CustomerEmail,
    string? CustomerPhone,
    string Description,
    decimal Amount,
    string Currency,
    string SuccessUrl,
    string CancelUrl,
    string? ExternalReference,
    string? TenantApiKey,
    string? TenantSecretKey,
    Guid PaymentId,
    string? WebhookUrl);

public sealed record InfinitePayCheckoutResult(
    string ExternalCheckoutId,
    string CheckoutUrl,
    string RawResponse);

public sealed record InfinitePayWebhookParsedEvent(
    string ProviderCode,
    string ExternalEventId,
    string? ExternalCheckoutId,
    string? ExternalPaymentId,
    string? Status,
    decimal? Amount,
    DateTime? PaidAtUtc,
    string RawPayload);

public interface IInfinitePayCheckoutService
{
    Task<InfinitePayCheckoutResult> CreatePlatformCheckoutAsync(CreateInfinitePayPlatformCheckoutCommand command, CancellationToken cancellationToken = default);

    Task<InfinitePayCheckoutResult> CreateTenantCheckoutAsync(CreateInfinitePayTenantCheckoutCommand command, CancellationToken cancellationToken = default);
}

public interface IInfinitePayWebhookService
{
    Task<InfinitePayWebhookParsedEvent?> ParseAsync(string payload, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken = default);

    Task<bool> ValidateSignatureAsync(string payload, IReadOnlyDictionary<string, string> headers, string? webhookSecret, CancellationToken cancellationToken = default);
}
