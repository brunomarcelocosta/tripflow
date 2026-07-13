using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IGetCurrentTenantFinancialSettingsUseCase
{
    Task<Result<TenantFinancialSettingsResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
