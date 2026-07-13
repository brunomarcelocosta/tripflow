using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Miles;

[ApiController]
[Route("api/loyalty-programs")]
[Authorize]
public sealed class LoyaltyProgramsController(
    IGetLoyaltyProgramsUseCase getLoyaltyProgramsUseCase,
    IGetLoyaltyProgramByIdUseCase getLoyaltyProgramByIdUseCase,
    ICreateLoyaltyProgramUseCase createLoyaltyProgramUseCase,
    IUpdateLoyaltyProgramUseCase updateLoyaltyProgramUseCase,
    IActivateLoyaltyProgramUseCase activateLoyaltyProgramUseCase,
    IInactivateLoyaltyProgramUseCase inactivateLoyaltyProgramUseCase,
    IDeleteLoyaltyProgramUseCase deleteLoyaltyProgramUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetAll([FromQuery] LoyaltyProgramFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getLoyaltyProgramsUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getLoyaltyProgramByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Create([FromBody] CreateLoyaltyProgramRequest request, CancellationToken cancellationToken)
    {
        var result = await createLoyaltyProgramUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateLoyaltyProgramRequest request, CancellationToken cancellationToken)
    {
        var result = await updateLoyaltyProgramUseCase.ExecuteAsync(id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await activateLoyaltyProgramUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/inactivate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Inactivate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await inactivateLoyaltyProgramUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteLoyaltyProgramUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
