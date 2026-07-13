using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;

namespace Tripflow.Application.UseCases.Tenants.Interfaces;

public interface IUpdateTenantFinancialSettingsUseCase
{
    Task<Result<TenantFinancialSettingsResponse>> ExecuteAsync(
        UpdateTenantFinancialSettingsRequest request,
        CancellationToken cancellationToken = default);
}
