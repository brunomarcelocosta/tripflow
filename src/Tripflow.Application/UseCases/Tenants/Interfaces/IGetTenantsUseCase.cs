using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IGetTenantsUseCase
{
    Task<Result<PagedResponse<TenantResponse>>> ExecuteAsync(TenantFilterRequest request, CancellationToken cancellationToken);
}
