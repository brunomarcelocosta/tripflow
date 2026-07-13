using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes/{quoteId:guid}/itinerary")]
[Authorize]
public sealed class QuoteItineraryController(
    IGetQuoteItineraryUseCase getItineraryUseCase,
    IUpdateQuoteItineraryUseCase updateItineraryUseCase,
    ICreateItineraryStopUseCase createStopUseCase,
    IUpdateItineraryStopUseCase updateStopUseCase,
    IDeleteItineraryStopUseCase deleteStopUseCase,
    IReorderItineraryStopsUseCase reorderStopsUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> Get([FromRoute] Guid quoteId, CancellationToken cancellationToken)
    {
        var result = await getItineraryUseCase.ExecuteAsync(quoteId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid quoteId, [FromBody] UpdateItineraryRequest request, CancellationToken cancellationToken)
    {
        var result = await updateItineraryUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("stops")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> CreateStop([FromRoute] Guid quoteId, [FromBody] CreateItineraryStopRequest request, CancellationToken cancellationToken)
    {
        var result = await createStopUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("stops/reorder")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> ReorderStops([FromRoute] Guid quoteId, [FromBody] ReorderItineraryStopsRequest request, CancellationToken cancellationToken)
    {
        var result = await reorderStopsUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("stops/{stopId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> UpdateStop([FromRoute] Guid quoteId, [FromRoute] Guid stopId, [FromBody] UpdateItineraryStopRequest request, CancellationToken cancellationToken)
    {
        var result = await updateStopUseCase.ExecuteAsync(quoteId, stopId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("stops/{stopId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> DeleteStop([FromRoute] Guid quoteId, [FromRoute] Guid stopId, CancellationToken cancellationToken)
    {
        var result = await deleteStopUseCase.ExecuteAsync(quoteId, stopId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
