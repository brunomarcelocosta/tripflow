using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class GetTenantPaymentProvidersUseCase(
    ITenantPaymentProviderRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetTenantPaymentProvidersUseCase
{
    public async Task<Result<IEnumerable<TenantPaymentProviderResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<TenantPaymentProviderResponse>>.Forbidden();

        if (!await HasPaymentsOrSettingsReadAsync(cancellationToken))
            return Result<IEnumerable<TenantPaymentProviderResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var providers = await repository.GetByTenantIdAsync(tenantId, cancellationToken);
        var mapped = providers.Select(mapper.Map<TenantPaymentProviderResponse>).ToList();
        return Result<IEnumerable<TenantPaymentProviderResponse>>.Ok(mapped);
    }

    private async Task<bool> HasPaymentsOrSettingsReadAsync(CancellationToken cancellationToken)
        => await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken)
           || await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken);
}
