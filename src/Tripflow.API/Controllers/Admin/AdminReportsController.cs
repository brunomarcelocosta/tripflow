using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Admin;

[ApiController]
[Route("api/admin/reports")]
[Authorize]
public sealed class AdminReportsController(
    IGetAdminTenantReportUseCase getAdminTenantReportUseCase,
    IGetAdminUsageReportUseCase getAdminUsageReportUseCase,
    IGetAdminPaymentReportUseCase getAdminPaymentReportUseCase,
    IGetAdminProposalReportUseCase getAdminProposalReportUseCase,
    IGetAdminSubscriptionReportUseCase getAdminSubscriptionReportUseCase) : ControllerBase
{
    [HttpGet("tenants")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.ReportsRead)]
    public async Task<IActionResult> GetTenants([FromQuery] AdminReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminTenantReportUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("usage")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.ReportsRead)]
    public async Task<IActionResult> GetUsage([FromQuery] AdminReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminUsageReportUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("payments")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.ReportsRead)]
    public async Task<IActionResult> GetPayments([FromQuery] AdminReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminPaymentReportUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("proposals")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.ReportsRead)]
    public async Task<IActionResult> GetProposals([FromQuery] AdminReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminProposalReportUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("subscriptions")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.ReportsRead)]
    public async Task<IActionResult> GetSubscriptions([FromQuery] AdminReportFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminSubscriptionReportUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
