using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Miles;

[ApiController]
[Route("api/customers/{customerId:guid}/loyalty-accounts/{accountId:guid}/expiration-batches")]
[Authorize]
public sealed class MilesExpirationBatchesController(
    IGetMilesExpirationBatchesUseCase getMilesExpirationBatchesUseCase,
    ICreateMilesExpirationBatchUseCase createMilesExpirationBatchUseCase,
    IUpdateMilesExpirationBatchUseCase updateMilesExpirationBatchUseCase,
    ICancelMilesExpirationBatchUseCase cancelMilesExpirationBatchUseCase,
    IMarkMilesExpirationBatchAsExpiredUseCase markMilesExpirationBatchAsExpiredUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid accountId, [FromQuery] MilesExpirationBatchFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getMilesExpirationBatchesUseCase.ExecuteAsync(accountId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid accountId, [FromBody] CreateMilesExpirationBatchRequest request, CancellationToken cancellationToken)
    {
        var fixedRequest = request with { CustomerLoyaltyAccountId = accountId };
        var result = await createMilesExpirationBatchUseCase.ExecuteAsync(fixedRequest, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid accountId, [FromRoute] Guid id, [FromBody] UpdateMilesExpirationBatchRequest request, CancellationToken cancellationToken)
    {
        var result = await updateMilesExpirationBatchUseCase.ExecuteAsync(accountId, id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Cancel([FromRoute] Guid accountId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await cancelMilesExpirationBatchUseCase.ExecuteAsync(accountId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/mark-as-expired")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> MarkAsExpired([FromRoute] Guid accountId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await markMilesExpirationBatchAsExpiredUseCase.ExecuteAsync(accountId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
