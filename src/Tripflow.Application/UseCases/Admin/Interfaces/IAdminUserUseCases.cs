using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;

namespace Tripflow.Application.UseCases.Admin.Interfaces;

public interface IGetAdminTenantUsersUseCase
{
    Task<Result<PagedResponse<AdminUserResponse>>> ExecuteAsync(Guid tenantId, AdminUserFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminUsersUseCase
{
    Task<Result<PagedResponse<AdminUserResponse>>> ExecuteAsync(AdminUserFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminUserByIdUseCase
{
    Task<Result<AdminUserDetailResponse?>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IUpdateAdminUserUseCase
{
    Task<Result<AdminUserResponse?>> ExecuteAsync(Guid userId, UpdateAdminUserRequest request, CancellationToken cancellationToken = default);
}

public interface ISetAdminUserPasswordUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid userId, SetAdminUserPasswordRequest request, CancellationToken cancellationToken = default);
}

public interface IActivateAdminUserUseCase
{
    Task<Result<AdminUserResponse?>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IBlockAdminUserUseCase
{
    Task<Result<AdminUserResponse?>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IRemoveAdminUserUseCase
{
    Task<Result<AdminUserResponse?>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default);
}
