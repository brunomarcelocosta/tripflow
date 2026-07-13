using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class CancelPaymentUseCase(
    IPaymentRepository paymentRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CancelPaymentUseCase> logger) : ICancelPaymentUseCase
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

        if (!payment.CanBeCancelled())
            return Result<bool>.Failure("Pagamento não pode ser cancelado no status atual.");

        try
        {
            payment.Cancel(updatedBy);
            await paymentRepository.UpdateAsync(payment, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("CancelPaymentUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao cancelar pagamento.");
        }
    }
}
