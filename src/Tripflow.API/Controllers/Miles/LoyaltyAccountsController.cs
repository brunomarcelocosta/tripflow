using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Miles;

[ApiController]
[Route("api/loyalty-accounts")]
[Authorize]
public sealed class LoyaltyAccountsController(
    IGetLoyaltyAccountByIdUseCase getLoyaltyAccountByIdUseCase) : ControllerBase
{
    [HttpGet("{accountId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid accountId, CancellationToken cancellationToken)
    {
        var result = await getLoyaltyAccountByIdUseCase.ExecuteAsync(accountId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }
}
