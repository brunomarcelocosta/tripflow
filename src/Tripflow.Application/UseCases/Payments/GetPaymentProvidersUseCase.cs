using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class GetPaymentProvidersUseCase(
    IPaymentProviderRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetPaymentProvidersUseCase
{
    public async Task<Result<IEnumerable<PaymentProviderResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<PaymentProviderResponse>>.Forbidden();

        if (!await HasPaymentsOrSettingsReadAsync(cancellationToken))
            return Result<IEnumerable<PaymentProviderResponse>>.Forbidden();

        var providers = await repository.GetActiveAsync(cancellationToken);
        var mapped = providers.Select(mapper.Map<PaymentProviderResponse>).ToList();
        return Result<IEnumerable<PaymentProviderResponse>>.Ok(mapped);
    }

    private async Task<bool> HasPaymentsOrSettingsReadAsync(CancellationToken cancellationToken)
        => await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken)
           || await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken);
}
