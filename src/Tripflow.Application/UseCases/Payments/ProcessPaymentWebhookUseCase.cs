using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.UseCases.Payments;

public class ProcessPaymentWebhookUseCase(
    IPaymentWebhookEventRepository webhookEventRepository,
    IPaymentWebhookParser webhookParser,
    IPaymentRepository paymentRepository,
    IProposalRepository proposalRepository,
    IQuotePricingOptionRepository pricingRepository,
    IFinancialTransactionService financialTransactionService,
    ILogger<ProcessPaymentWebhookUseCase> logger) : IProcessPaymentWebhookUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid webhookEventId, CancellationToken cancellationToken = default)
    {
        var webhookEvent = await webhookEventRepository.GetUnprocessedAsync(webhookEventId, cancellationToken);
        if (webhookEvent is null)
            return Result<bool>.Ok(true);

        var parsed = await webhookParser.ParseAsync(webhookEvent.ProviderCode, webhookEvent.Payload, cancellationToken);
        if (parsed is null)
        {
            webhookEvent.MarkAsProcessed();
            await webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);
            return Result<bool>.Failure("Não foi possível interpretar o evento de webhook.");
        }

        var externalPaymentId = parsed.ExternalPaymentId ?? webhookEvent.ExternalPaymentId;
        if (string.IsNullOrWhiteSpace(externalPaymentId))
        {
            webhookEvent.MarkAsProcessed();
            await webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);
            return Result<bool>.Ok(true);
        }

        var payment = await paymentRepository.GetTrackedByExternalPaymentIdAsync(externalPaymentId, cancellationToken);
        if (payment is null)
        {
            logger.LogWarning(
                "ProcessPaymentWebhookUseCase | Pagamento não encontrado | ExternalPaymentId={ExternalPaymentId}",
                externalPaymentId);

            webhookEvent.MarkAsProcessed();
            await webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);
            return Result<bool>.Ok(true);
        }

        try
        {
            const string updatedBy = "webhook";

            if (parsed.PaymentStatus == PaymentStatus.Paid)
            {
                if (payment.Status != PaymentStatus.Paid)
                {
                    payment.MarkAsPaid(updatedBy);
                    await paymentRepository.UpdateAsync(payment, cancellationToken);
                }

                var agencyCost = await PaymentAgencyCostHelper.ResolveAsync(payment, proposalRepository, pricingRepository, cancellationToken);
                await financialTransactionService.RegisterSaleAsync(payment, agencyCost, updatedBy, cancellationToken);
            }
            else if (parsed.PaymentStatus.HasValue && parsed.PaymentStatus.Value != payment.Status)
            {
                ApplyStatus(payment, parsed.PaymentStatus.Value, updatedBy);
                await paymentRepository.UpdateAsync(payment, cancellationToken);
            }

            webhookEvent.MarkAsProcessed();
            await webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("ProcessPaymentWebhookUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao processar webhook de pagamento.");
        }
    }

    private static void ApplyStatus(Domain.Entities.Payments.Payment payment, PaymentStatus status, string updatedBy)
    {
        switch (status)
        {
            case PaymentStatus.Cancelled:
                payment.Cancel(updatedBy);
                break;
            case PaymentStatus.Refunded:
                payment.Refund(updatedBy);
                break;
            case PaymentStatus.Failed:
                payment.Fail(updatedBy);
                break;
            case PaymentStatus.Expired:
                payment.Expire(updatedBy);
                break;
        }
    }
}
