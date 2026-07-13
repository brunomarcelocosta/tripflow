using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.UseCases.Payments;

public class ReceivePaymentWebhookUseCase(
    IPaymentWebhookParser webhookParser,
    IPaymentWebhookEventRepository webhookEventRepository,
    IProcessPaymentWebhookUseCase processPaymentWebhookUseCase,
    ILogger<ReceivePaymentWebhookUseCase> logger) : IReceivePaymentWebhookUseCase
{
    public async Task<Result<bool>> ExecuteAsync(string providerCode, string payload, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerCode))
            return Result<bool>.Failure("Código do provedor é obrigatório.");

        if (string.IsNullOrWhiteSpace(payload))
            return Result<bool>.Failure("Payload do webhook é obrigatório.");

        var parsed = await webhookParser.ParseAsync(providerCode, payload, cancellationToken);
        if (parsed is null)
            return Result<bool>.Failure("Não foi possível interpretar o webhook recebido.");

        if (await webhookEventRepository.ExistsByProviderAndExternalEventIdAsync(providerCode, parsed.ExternalEventId, cancellationToken))
            return Result<bool>.Ok(true);

        try
        {
            var webhookEvent = new PaymentWebhookEvent(
                providerCode,
                parsed.ExternalEventId,
                parsed.ExternalPaymentId,
                payload);

            await webhookEventRepository.AddAsync(webhookEvent, cancellationToken);

            var processResult = await processPaymentWebhookUseCase.ExecuteAsync(webhookEvent.Id, cancellationToken);
            if (!processResult.Success)
                return Result<bool>.Failure(processResult.Error ?? "Erro ao processar webhook.");

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("ReceivePaymentWebhookUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao receber webhook de pagamento.");
        }
    }
}
