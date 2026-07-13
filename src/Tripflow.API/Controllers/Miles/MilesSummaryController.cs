using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Miles;

[ApiController]
[Route("api")]
[Authorize]
public sealed class MilesSummaryController(
    IGetCustomerMilesSummaryUseCase getCustomerMilesSummaryUseCase,
    IGetMilesSummaryUseCase getMilesSummaryUseCase) : ControllerBase
{
    [HttpGet("customers/{customerId:guid}/miles-summary")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetCustomerSummary([FromRoute] Guid customerId, CancellationToken cancellationToken)
    {
        var result = await getCustomerMilesSummaryUseCase.ExecuteAsync(customerId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("miles-summary")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await getMilesSummaryUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
