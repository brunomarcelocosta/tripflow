using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class CancelPaymentLinkUseCase(
    IPaymentRepository paymentRepository,
    IPaymentLinkRepository linkRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CancelPaymentLinkUseCase> logger) : ICancelPaymentLinkUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid paymentId, Guid linkId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var payment = await paymentRepository.GetByIdAndTenantAsync(paymentId, tenantId, cancellationToken);
        if (payment is null)
            return Result<bool>.Failure("Pagamento não encontrado.");

        var link = await linkRepository.GetTrackedByIdAndPaymentAsync(linkId, paymentId, tenantId, cancellationToken);
        if (link is null)
            return Result<bool>.Failure("Link de pagamento não encontrado.");

        if (link.Status is PaymentLinkStatus.Cancelled or PaymentLinkStatus.Paid)
            return Result<bool>.Failure("Link de pagamento não pode ser cancelado no status atual.");

        try
        {
            link.Cancel(updatedBy);
            await linkRepository.UpdateAsync(link, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("CancelPaymentLinkUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao cancelar link de pagamento.");
        }
    }
}
