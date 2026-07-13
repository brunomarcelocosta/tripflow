using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IUpdateTenantUseCase
{
    Task<Result<TenantResponse?>> ExecuteAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default);
}
