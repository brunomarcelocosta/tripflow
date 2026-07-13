using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IGetCurrentTenantCommercialSettingsUseCase
{
    Task<Result<TenantCommercialSettingsResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
