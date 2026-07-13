using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;

namespace Tripflow.Application.UseCases.Admin.Interfaces;

public interface IGetAdminTenantsUseCase
{
    Task<Result<PagedResponse<AdminTenantResponse>>> ExecuteAsync(AdminTenantFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminTenantByIdUseCase
{
    Task<Result<AdminTenantDetailResponse?>> ExecuteAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface IActivateAdminTenantUseCase
{
    Task<Result<AdminTenantResponse?>> ExecuteAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface ISuspendAdminTenantUseCase
{
    Task<Result<AdminTenantResponse?>> ExecuteAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface ICancelAdminTenantUseCase
{
    Task<Result<AdminTenantResponse?>> ExecuteAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface IUpdateAdminTenantUseCase
{
    Task<Result<AdminTenantResponse?>> ExecuteAsync(Guid tenantId, UpdateTenantRequest request, CancellationToken cancellationToken = default);
}
