using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Roles;
using Tripflow.Application.UseCases.Roles.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Roles;

public sealed class GetRolesUseCase(
    ITenantContext tenantContext,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IRoleRepository roleRepository,
    IMapper mapper) : IGetRolesUseCase
{
    public async Task<Result<List<RoleResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<List<RoleResponse>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<List<RoleResponse>>.Failure("Tenant não resolvido para o usuário atual.");

        var canRead = await userPermissionService.HasPermissionAsync(
            TripflowDbSeedData.Permissions.UsersRead,
            cancellationToken);

        var canManage = await userPermissionService.HasPermissionAsync(
            TripflowDbSeedData.Permissions.UsersManage,
            cancellationToken);

        if (!canRead && !canManage)
            return Result<List<RoleResponse>>.Forbidden();

        var roles = await roleRepository.GetAllByTenantAsync(tenantContext.TenantId.Value, cancellationToken);

        var response = mapper.Map<List<RoleResponse>>(roles);

        return Result<List<RoleResponse>>.Ok(response);
    }
}
