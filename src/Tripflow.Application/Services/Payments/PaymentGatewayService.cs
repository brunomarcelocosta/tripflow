using Microsoft.Extensions.Logging;

namespace Tripflow.Application.Services.Payments;

public class PaymentGatewayService(
    ManualPaymentGatewayService manualGateway,
    AsaasPaymentGatewayService asaasGateway,
    InfinitePayPaymentGatewayService infinitePayGateway,
    ILogger<PaymentGatewayService> logger) : IPaymentGatewayService
{
    public Task<CreatePaymentLinkResult> CreatePaymentLinkAsync(CreatePaymentLinkCommand command, CancellationToken cancellationToken = default)
    {
        var providerCode = command.ProviderCode?.Trim().ToLowerInvariant() ?? "manual";

        logger.LogInformation(
            "PaymentGatewayService | Provider={ProviderCode} | PaymentId={PaymentId}",
            providerCode,
            command.PaymentId);

        return providerCode switch
        {
            "asaas" => asaasGateway.CreatePaymentLinkAsync(command, cancellationToken),
            "infinitepay" => infinitePayGateway.CreatePaymentLinkAsync(command, cancellationToken),
            _ => manualGateway.CreatePaymentLinkAsync(command, cancellationToken)
        };
    }
}
