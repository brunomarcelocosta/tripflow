using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Travelers;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Travelers;

public class GetTravelerByIdUseCase(
    ITravelerRepository travelerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper) : IGetTravelerByIdUseCase
{
    public async Task<Result<TravelerResponse?>> ExecuteAsync(Guid travelerId, Guid? customerId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TravelerResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TravelerResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TravelersRead,
                cancellationToken))
        {
            return Result<TravelerResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;

        var traveler = customerId.HasValue
            ? await travelerRepository.GetByCustomerAndTenantAsync(travelerId, customerId.Value, tenantId, cancellationToken)
            : await travelerRepository.GetByIdAndTenantAsync(travelerId, tenantId, cancellationToken);

        if (traveler is null)
            return Result<TravelerResponse?>.Failure("Viajante não encontrado.");

        var response = mapper.Map<TravelerResponse>(traveler);

        return Result<TravelerResponse?>.Ok(response);
    }
}
