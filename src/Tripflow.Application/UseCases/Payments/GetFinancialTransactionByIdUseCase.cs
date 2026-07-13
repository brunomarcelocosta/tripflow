using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class GetFinancialTransactionByIdUseCase(
    IFinancialTransactionRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetFinancialTransactionByIdUseCase
{
    public async Task<Result<FinancialTransactionResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<FinancialTransactionResponse?>.Forbidden();

        if (!await HasPaymentsOrReportsReadAsync(cancellationToken))
            return Result<FinancialTransactionResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var transaction = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (transaction is null)
            return Result<FinancialTransactionResponse?>.Ok(null);

        return Result<FinancialTransactionResponse?>.Ok(mapper.Map<FinancialTransactionResponse>(transaction));
    }

    private async Task<bool> HasPaymentsOrReportsReadAsync(CancellationToken cancellationToken)
        => await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken)
           || await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ReportsRead, cancellationToken);
}
