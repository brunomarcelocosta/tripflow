using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class SetDefaultTenantPaymentProviderUseCase(
    ITenantPaymentProviderRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<SetDefaultTenantPaymentProviderUseCase> logger) : ISetDefaultTenantPaymentProviderUseCase
{
    public async Task<Result<TenantPaymentProviderResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantPaymentProviderResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantPaymentProviderResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var entity = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (entity is null)
            return Result<TenantPaymentProviderResponse?>.Failure("Provedor de pagamento não encontrado.");

        try
        {
            var all = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);
            foreach (var item in all)
                item.SetDefault(item.Id == id, updatedBy);

            await repository.UpdateAsync(entity, cancellationToken);

            var fresh = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
            return Result<TenantPaymentProviderResponse?>.Ok(mapper.Map<TenantPaymentProviderResponse>(fresh));
        }
        catch (Exception ex)
        {
            logger.LogError("SetDefaultTenantPaymentProviderUseCase | Erro | {Message}", ex.Message);
            return Result<TenantPaymentProviderResponse?>.Failure("Erro inesperado ao definir provedor padrão.");
        }
    }
}
