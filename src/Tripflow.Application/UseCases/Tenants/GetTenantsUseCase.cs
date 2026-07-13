using AutoMapper;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public class GetTenantsUseCase(
    ITenantRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IMapper mapper) : IGetTenantsUseCase
{
    public async Task<Result<PagedResponse<TenantResponse>>> ExecuteAsync(TenantFilterRequest request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<TenantResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TenantsRead,
                cancellationToken))
        {
            return Result<PagedResponse<TenantResponse>>.Forbidden();
        }

        var orderBy = BuildOrderByHelper.BuildOrderBy<Tenant>(request.SortBy);

        var filter = request.ToExpression();

        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);

        var mapped = mapper.Map<List<TenantResponse>>(paged.Items);

        var response = new PagedResponse<TenantResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages);

        return Result<PagedResponse<TenantResponse>>.Ok(response);
    }
}
