using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Payments.InfinitePay;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Payments;

public class InfinitePayPaymentGatewayService(
    IInfinitePayCheckoutService infinitePayCheckoutService,
    IOptions<InfinitePayOptions> options,
    ILogger<InfinitePayPaymentGatewayService> logger) : IPaymentGatewayService
{
    public async Task<CreatePaymentLinkResult> CreatePaymentLinkAsync(CreatePaymentLinkCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.ApiKey))
            throw new InvalidOperationException("Handle InfinitePay não configurado para o tenant.");

        var description = command.Description ?? $"Pagamento TripFlow #{command.PaymentId:N}";
        var redirectUrl = command.RedirectUrl ?? options.Value.CheckoutSuccessUrl;
        if (string.IsNullOrWhiteSpace(redirectUrl))
            redirectUrl = $"{options.Value.ApiPublicBaseUrl.TrimEnd('/')}/checkout/success";

        var externalReference = $"tripflow:tenant:{command.TenantId}:proposal:{command.ProposalId}:payment:{command.PaymentId}";

        try
        {
            var result = await infinitePayCheckoutService.CreateTenantCheckoutAsync(
                new CreateInfinitePayTenantCheckoutCommand(
                    command.TenantId,
                    command.CustomerName ?? "Cliente",
                    command.CustomerEmail ?? "cliente@tripflow.local",
                    command.CustomerPhone,
                    description,
                    command.GrossAmount,
                    options.Value.PlatformDefaultCurrency,
                    redirectUrl,
                    options.Value.CheckoutCancelUrl,
                    externalReference,
                    command.ApiKey,
                    null,
                    command.PaymentId,
                    command.WebhookUrl),
                cancellationToken);

            return new CreatePaymentLinkResult(result.ExternalCheckoutId, result.CheckoutUrl, command.PaymentId.ToString("N"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "InfinitePayPaymentGatewayService | Erro ao criar link | PaymentId={PaymentId}", command.PaymentId);
            throw;
        }
    }
}
