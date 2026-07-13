using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Subscriptions;

[ApiController]
[Route("api/tenant-usage")]
[Authorize]
public sealed class TenantUsageController(
    IGetCurrentTenantUsagesUseCase getCurrentTenantUsagesUseCase,
    IGetTenantUsageByTypeUseCase getTenantUsageByTypeUseCase,
    IIncrementTenantUsageUseCase incrementTenantUsageUseCase,
    IDecrementTenantUsageUseCase decrementTenantUsageUseCase) : ControllerBase
{
    [HttpGet("current")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await getCurrentTenantUsagesUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{usageType}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetByType([FromRoute] string usageType, CancellationToken cancellationToken)
    {
        var result = await getTenantUsageByTypeUseCase.ExecuteAsync(usageType, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{usageType}/increment")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Increment([FromRoute] string usageType, [FromQuery] int amount = 1, CancellationToken cancellationToken = default)
    {
        var result = await incrementTenantUsageUseCase.ExecuteAsync(usageType, amount, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{usageType}/decrement")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Decrement([FromRoute] string usageType, [FromQuery] int amount = 1, CancellationToken cancellationToken = default)
    {
        var result = await decrementTenantUsageUseCase.ExecuteAsync(usageType, amount, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
