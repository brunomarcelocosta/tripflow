using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Miles;

[ApiController]
[Route("api/customers/{customerId:guid}/loyalty-accounts/{accountId:guid}/transactions")]
[Authorize]
public sealed class MilesTransactionsController(
    IGetMilesTransactionsUseCase getMilesTransactionsUseCase,
    IGetMilesTransactionByIdUseCase getMilesTransactionByIdUseCase,
    ICreateMilesTransactionUseCase createMilesTransactionUseCase,
    IDeleteMilesTransactionUseCase deleteMilesTransactionUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid accountId, [FromQuery] MilesTransactionFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getMilesTransactionsUseCase.ExecuteAsync(accountId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid accountId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getMilesTransactionByIdUseCase.ExecuteAsync(accountId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid accountId, [FromBody] CreateMilesTransactionRequest request, CancellationToken cancellationToken)
    {
        var fixedRequest = request with { CustomerLoyaltyAccountId = accountId };
        var result = await createMilesTransactionUseCase.ExecuteAsync(fixedRequest, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid accountId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteMilesTransactionUseCase.ExecuteAsync(accountId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
