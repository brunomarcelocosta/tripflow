using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Payments;

public sealed record CreatePaymentLinkCommand(
    Guid TenantId,
    Guid PaymentId,
    string ProviderCode,
    string? ApiKey,
    decimal GrossAmount,
    PaymentMethod PaymentMethod,
    int? Installments,
    DateTime? ExpiresAtUtc,
    string? Description = null,
    string? CustomerName = null,
    string? CustomerEmail = null,
    string? CustomerPhone = null,
    string? RedirectUrl = null,
    string? WebhookUrl = null,
    Guid? ProposalId = null);

public sealed record CreatePaymentLinkResult(
    string ExternalLinkId,
    string Url,
    string? ExternalPaymentReference = null);

public interface IPaymentGatewayService
{
    Task<CreatePaymentLinkResult> CreatePaymentLinkAsync(CreatePaymentLinkCommand command, CancellationToken cancellationToken = default);
}
