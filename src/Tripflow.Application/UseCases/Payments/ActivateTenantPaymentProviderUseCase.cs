using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class ActivateTenantPaymentProviderUseCase(
    ITenantPaymentProviderRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<ActivateTenantPaymentProviderUseCase> logger) : IActivateTenantPaymentProviderUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var entity = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure("Provedor de pagamento não encontrado.");

        try
        {
            entity.Activate(updatedBy);
            await repository.UpdateAsync(entity, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("ActivateTenantPaymentProviderUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao ativar provedor de pagamento.");
        }
    }
}
