using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize]
public sealed class AdminDashboardController(
    IGetAdminDashboardOverviewUseCase getAdminDashboardOverviewUseCase) : ControllerBase
{
    [HttpGet("overview")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.DashboardRead)]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var result = await getAdminDashboardOverviewUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
