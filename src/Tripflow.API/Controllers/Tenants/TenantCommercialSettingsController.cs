using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Tenants;

[ApiController]
[Route("api/tenant-commercial-settings")]
[Authorize]
public sealed class TenantCommercialSettingsController(
    IGetCurrentTenantCommercialSettingsUseCase getCurrentUseCase,
    IUpdateTenantCommercialSettingsUseCase updateUseCase) : ControllerBase
{
    [HttpGet("current")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await getCurrentUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("current")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> UpdateCurrent(
        [FromBody] UpdateTenantCommercialSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
