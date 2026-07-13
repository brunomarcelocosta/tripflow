using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Payments;

public sealed record ParsedPaymentWebhook(
    string ExternalEventId,
    string? ExternalPaymentId,
    PaymentStatus? PaymentStatus,
    decimal? GrossAmount,
    decimal? FeeAmount,
    decimal? NetAmount,
    DateTime? PaidAtUtc);

public interface IPaymentWebhookParser
{
    Task<ParsedPaymentWebhook?> ParseAsync(string providerCode, string payload, CancellationToken cancellationToken = default);
}
