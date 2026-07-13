using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IUpdateTenantBrandingUseCase
{
    Task<Result<TenantBrandingResponse>> ExecuteAsync(
        UpdateTenantBrandingRequest request,
        CancellationToken cancellationToken = default);
}
