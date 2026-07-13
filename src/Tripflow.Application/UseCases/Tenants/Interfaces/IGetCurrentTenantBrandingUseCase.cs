using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IGetCurrentTenantBrandingUseCase
{
    Task<Result<TenantBrandingResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
