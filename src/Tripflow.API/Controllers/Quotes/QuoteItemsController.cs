using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes/{quoteId:guid}/items")]
[Authorize]
public sealed class QuoteItemsController(
    IGetQuoteItemsUseCase getItemsUseCase,
    IGetQuoteItemByIdUseCase getItemByIdUseCase,
    ICreateQuoteItemUseCase createItemUseCase,
    IUpdateQuoteItemUseCase updateItemUseCase,
    IDeleteQuoteItemUseCase deleteItemUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid quoteId, CancellationToken cancellationToken)
    {
        var result = await getItemsUseCase.ExecuteAsync(quoteId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{itemId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var result = await getItemByIdUseCase.ExecuteAsync(quoteId, itemId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid quoteId, [FromBody] CreateQuoteItemRequest request, CancellationToken cancellationToken)
    {
        var result = await createItemUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{itemId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid quoteId, [FromRoute] Guid itemId, [FromBody] UpdateQuoteItemRequest request, CancellationToken cancellationToken)
    {
        var result = await updateItemUseCase.ExecuteAsync(quoteId, itemId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{itemId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid quoteId, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var result = await deleteItemUseCase.ExecuteAsync(quoteId, itemId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
