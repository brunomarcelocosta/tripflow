using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Subscriptions;

[ApiController]
[Route("api/subscription-plans/{planId:guid}/features")]
[Authorize]
public sealed class PlanFeaturesController(
    IGetPlanFeaturesUseCase getPlanFeaturesUseCase,
    IUpdatePlanFeaturesUseCase updatePlanFeaturesUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> Get([FromRoute] Guid planId, CancellationToken cancellationToken)
    {
        var result = await getPlanFeaturesUseCase.ExecuteAsync(planId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid planId,
        [FromBody] UpdatePlanFeaturesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updatePlanFeaturesUseCase.ExecuteAsync(planId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
