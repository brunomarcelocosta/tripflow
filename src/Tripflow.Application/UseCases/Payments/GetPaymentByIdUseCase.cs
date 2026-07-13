using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class GetPaymentByIdUseCase(
    IPaymentRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetPaymentByIdUseCase
{
    public async Task<Result<PaymentResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PaymentResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken))
            return Result<PaymentResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var payment = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (payment is null)
            return Result<PaymentResponse?>.Ok(null);

        return Result<PaymentResponse?>.Ok(mapper.Map<PaymentResponse>(payment));
    }
}
