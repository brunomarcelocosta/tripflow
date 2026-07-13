using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Subscriptions;

[ApiController]
[Route("api/subscription-plans")]
[Authorize]
public sealed class SubscriptionPlansController(
    IGetSubscriptionPlansUseCase getSubscriptionPlansUseCase,
    IGetSubscriptionPlanByIdUseCase getSubscriptionPlanByIdUseCase,
    ICreateSubscriptionPlanUseCase createSubscriptionPlanUseCase,
    IUpdateSubscriptionPlanUseCase updateSubscriptionPlanUseCase,
    IActivateSubscriptionPlanUseCase activateSubscriptionPlanUseCase,
    IInactivateSubscriptionPlanUseCase inactivateSubscriptionPlanUseCase,
    IDeprecateSubscriptionPlanUseCase deprecateSubscriptionPlanUseCase,
    IDeleteSubscriptionPlanUseCase deleteSubscriptionPlanUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetAll([FromQuery] SubscriptionPlanFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getSubscriptionPlansUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getSubscriptionPlanByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionPlanRequest request, CancellationToken cancellationToken)
    {
        var result = await createSubscriptionPlanUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSubscriptionPlanRequest request, CancellationToken cancellationToken)
    {
        var result = await updateSubscriptionPlanUseCase.ExecuteAsync(id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await activateSubscriptionPlanUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/inactivate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Inactivate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await inactivateSubscriptionPlanUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/deprecate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Deprecate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deprecateSubscriptionPlanUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteSubscriptionPlanUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
