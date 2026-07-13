using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Admin;

[ApiController]
[Route("api/admin/support/sessions")]
[Authorize]
public sealed class AdminSupportController(
    ICreateSupportSessionUseCase createSupportSessionUseCase,
    IGetCurrentSupportSessionUseCase getCurrentSupportSessionUseCase,
    IEndCurrentSupportSessionUseCase endCurrentSupportSessionUseCase,
    IGetSupportSessionsUseCase getSupportSessionsUseCase) : ControllerBase
{
    [HttpPost]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.SupportAccess)]
    public async Task<IActionResult> Create([FromBody] CreateSupportSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await createSupportSessionUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("current")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.SupportAccess)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await getCurrentSupportSessionUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpDelete("current")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.SupportAccess)]
    public async Task<IActionResult> EndCurrent(CancellationToken cancellationToken)
    {
        var result = await endCurrentSupportSessionUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.SupportAccess)]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getSupportSessionsUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
