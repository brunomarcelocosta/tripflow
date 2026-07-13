using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class MarkPaymentAsPaidUseCase(
    IPaymentRepository paymentRepository,
    IProposalRepository proposalRepository,
    IQuotePricingOptionRepository pricingRepository,
    IFinancialTransactionService financialTransactionService,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<MarkPaymentAsPaidUseCase> logger) : IMarkPaymentAsPaidUseCase
{
    public async Task<Result<PaymentResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PaymentResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsWrite, cancellationToken))
            return Result<PaymentResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var payment = await paymentRepository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (payment is null)
            return Result<PaymentResponse?>.Failure("Pagamento não encontrado.");

        if (payment.Status == PaymentStatus.Paid)
            return Result<PaymentResponse?>.Failure("Pagamento já está marcado como pago.");

        if (payment.Status is PaymentStatus.Cancelled or PaymentStatus.Refunded)
            return Result<PaymentResponse?>.Failure("Pagamento não pode ser marcado como pago no status atual.");

        try
        {
            payment.MarkAsPaid(updatedBy);
            await paymentRepository.UpdateAsync(payment, cancellationToken);

            var agencyCost = await PaymentAgencyCostHelper.ResolveAsync(payment, proposalRepository, pricingRepository, cancellationToken);
            await financialTransactionService.RegisterSaleAsync(payment, agencyCost, updatedBy, cancellationToken);

            var fresh = await paymentRepository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
            return Result<PaymentResponse?>.Ok(mapper.Map<PaymentResponse>(fresh));
        }
        catch (Exception ex)
        {
            logger.LogError("MarkPaymentAsPaidUseCase | Erro | {Message}", ex.Message);
            return Result<PaymentResponse?>.Failure("Erro inesperado ao marcar pagamento como pago.");
        }
    }
}
