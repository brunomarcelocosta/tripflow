using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.UseCases.Platform.Interfaces;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Subscriptions;

[ApiController]
[Route("api/tenant-subscription")]
[Authorize]
public sealed class TenantSubscriptionController(
    IGetCurrentTenantSubscriptionUseCase getCurrentTenantSubscriptionUseCase,
    IGetTenantEntitlementsUseCase getTenantEntitlementsUseCase,
    IUpdateTenantSubscriptionUseCase updateTenantSubscriptionUseCase,
    IActivateTenantSubscriptionUseCase activateTenantSubscriptionUseCase,
    ISuspendTenantSubscriptionUseCase suspendTenantSubscriptionUseCase,
    ICancelTenantSubscriptionUseCase cancelTenantSubscriptionUseCase,
    IMarkTenantSubscriptionPastDueUseCase markTenantSubscriptionPastDueUseCase) : ControllerBase
{
    [HttpGet("entitlements")]
    public async Task<IActionResult> GetEntitlements(CancellationToken cancellationToken)
    {
        var result = await getTenantEntitlementsUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("current")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await getCurrentTenantSubscriptionUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPut]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Update([FromBody] UpdateTenantSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var result = await updateTenantSubscriptionUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("activate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Activate(CancellationToken cancellationToken)
    {
        var result = await activateTenantSubscriptionUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("suspend")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Suspend(CancellationToken cancellationToken)
    {
        var result = await suspendTenantSubscriptionUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("cancel")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Cancel(CancellationToken cancellationToken)
    {
        var result = await cancelTenantSubscriptionUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("past-due")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> MarkPastDue(CancellationToken cancellationToken)
    {
        var result = await markTenantSubscriptionPastDueUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
