using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Travelers;

[ApiController]
[Route("api/travelers")]
[Authorize]
public sealed class TravelersController(IGetTravelerByIdUseCase getTravelerByIdUseCase) : ControllerBase
{
    [HttpGet("{travelerId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.TravelersRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid travelerId, CancellationToken cancellationToken)
    {
        var result = await getTravelerByIdUseCase.ExecuteAsync(travelerId, customerId: null, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }
}
