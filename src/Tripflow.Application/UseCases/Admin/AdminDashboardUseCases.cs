using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.UseCases.Admin;

public sealed class GetAdminDashboardOverviewUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IUserContext userContext) : IGetAdminDashboardOverviewUseCase
{
    public async Task<Result<AdminDashboardOverviewResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminDashboardOverviewResponse>.Forbidden();

        var data = await platformAdminRepository.GetDashboardOverviewAsync(cancellationToken);
        return Result<AdminDashboardOverviewResponse>.Ok(AdminMappingHelper.MapDashboard(data));
    }
}
