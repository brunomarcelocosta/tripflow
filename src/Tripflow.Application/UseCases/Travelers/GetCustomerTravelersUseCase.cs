using AutoMapper;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Travelers;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Travelers;

public class GetCustomerTravelersUseCase(
    ICustomerRepository customerRepository,
    ITravelerRepository travelerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper) : IGetCustomerTravelersUseCase
{
    public async Task<Result<PagedResponse<TravelerResponse>>> ExecuteAsync(Guid customerId, TravelerFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<TravelerResponse>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<TravelerResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TravelersRead,
                cancellationToken))
        {
            return Result<PagedResponse<TravelerResponse>>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;

        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);

        if (customer is null)
            return Result<PagedResponse<TravelerResponse>>.Failure("Cliente não encontrado.");

        var orderBy = TravelerOrderByHelper.Build(request.SortBy);
        var filter = request.ToExpression(customerId);

        var paged = await travelerRepository.GetPagedAsync(
            filter,
            request.Page,
            request.PageSize,
            orderBy,
            request.SortDesc);

        var mapped = mapper.Map<List<TravelerResponse>>(paged.Items);

        var response = new PagedResponse<TravelerResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages);

        return Result<PagedResponse<TravelerResponse>>.Ok(response);
    }
}
