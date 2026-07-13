using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes/{quoteId:guid}/miles-options")]
[Authorize]
public sealed class MilesQuoteOptionsController(
    IGetMilesQuoteOptionsUseCase getOptionsUseCase,
    IGetMilesQuoteOptionByIdUseCase getOptionByIdUseCase,
    ICreateMilesQuoteOptionUseCase createOptionUseCase,
    IUpdateMilesQuoteOptionUseCase updateOptionUseCase,
    IDeleteMilesQuoteOptionUseCase deleteOptionUseCase,
    ISelectMilesQuoteOptionUseCase selectOptionUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid quoteId, CancellationToken cancellationToken)
    {
        var result = await getOptionsUseCase.ExecuteAsync(quoteId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{milesOptionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId, [FromRoute] Guid milesOptionId, CancellationToken cancellationToken)
    {
        var result = await getOptionByIdUseCase.ExecuteAsync(quoteId, milesOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid quoteId, [FromBody] CreateMilesQuoteOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await createOptionUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{milesOptionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid quoteId, [FromRoute] Guid milesOptionId, [FromBody] UpdateMilesQuoteOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await updateOptionUseCase.ExecuteAsync(quoteId, milesOptionId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{milesOptionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid quoteId, [FromRoute] Guid milesOptionId, CancellationToken cancellationToken)
    {
        var result = await deleteOptionUseCase.ExecuteAsync(quoteId, milesOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{milesOptionId:guid}/select")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Select([FromRoute] Guid quoteId, [FromRoute] Guid milesOptionId, CancellationToken cancellationToken)
    {
        var result = await selectOptionUseCase.ExecuteAsync(quoteId, milesOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
