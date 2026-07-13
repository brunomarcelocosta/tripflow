using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Admin;

namespace Tripflow.Application.UseCases.Admin.Interfaces;

public interface IGetAdminDashboardOverviewUseCase
{
    Task<Result<AdminDashboardOverviewResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
