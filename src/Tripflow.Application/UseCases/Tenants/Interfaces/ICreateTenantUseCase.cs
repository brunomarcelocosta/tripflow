using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface ICreateTenantUseCase
{
    Task<Result<Guid?>> ExecuteAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
}
