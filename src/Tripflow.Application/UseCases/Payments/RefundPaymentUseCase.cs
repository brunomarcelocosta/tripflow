using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class RefundPaymentUseCase(
    IPaymentRepository paymentRepository,
    IProposalRepository proposalRepository,
    IQuotePricingOptionRepository pricingRepository,
    IFinancialTransactionService financialTransactionService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<RefundPaymentUseCase> logger) : IRefundPaymentUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var payment = await paymentRepository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (payment is null)
            return Result<bool>.Failure("Pagamento não encontrado.");

        if (!payment.CanBeRefunded())
            return Result<bool>.Failure("Pagamento não pode ser estornado no status atual.");

        try
        {
            payment.Refund(updatedBy);
            await paymentRepository.UpdateAsync(payment, cancellationToken);

            var agencyCost = await PaymentAgencyCostHelper.ResolveAsync(payment, proposalRepository, pricingRepository, cancellationToken);
            await financialTransactionService.RegisterRefundAsync(payment, agencyCost, updatedBy, cancellationToken);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("RefundPaymentUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao estornar pagamento.");
        }
    }
}
