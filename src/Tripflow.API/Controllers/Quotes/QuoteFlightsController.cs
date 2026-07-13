using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes/{quoteId:guid}/flights")]
[Authorize]
public sealed class QuoteFlightsController(
    IGetQuoteFlightsUseCase getFlightsUseCase,
    IGetQuoteFlightByIdUseCase getFlightByIdUseCase,
    ICreateQuoteFlightItemUseCase createFlightUseCase,
    IUpdateQuoteFlightItemUseCase updateFlightUseCase,
    IDeleteQuoteFlightItemUseCase deleteFlightUseCase,
    ICreateFlightSegmentUseCase createSegmentUseCase,
    IUpdateFlightSegmentUseCase updateSegmentUseCase,
    IDeleteFlightSegmentUseCase deleteSegmentUseCase,
    IReorderFlightSegmentsUseCase reorderSegmentsUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid quoteId, CancellationToken cancellationToken)
    {
        var result = await getFlightsUseCase.ExecuteAsync(quoteId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{flightItemId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, CancellationToken cancellationToken)
    {
        var result = await getFlightByIdUseCase.ExecuteAsync(quoteId, flightItemId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid quoteId, [FromBody] CreateQuoteFlightItemRequest request, CancellationToken cancellationToken)
    {
        var result = await createFlightUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{flightItemId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, [FromBody] UpdateQuoteFlightItemRequest request, CancellationToken cancellationToken)
    {
        var result = await updateFlightUseCase.ExecuteAsync(quoteId, flightItemId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{flightItemId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, CancellationToken cancellationToken)
    {
        var result = await deleteFlightUseCase.ExecuteAsync(quoteId, flightItemId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{flightItemId:guid}/segments")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> CreateSegment([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, [FromBody] CreateFlightSegmentRequest request, CancellationToken cancellationToken)
    {
        var result = await createSegmentUseCase.ExecuteAsync(quoteId, flightItemId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{flightItemId:guid}/segments/reorder")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> ReorderSegments([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, [FromBody] ReorderFlightSegmentsRequest request, CancellationToken cancellationToken)
    {
        var result = await reorderSegmentsUseCase.ExecuteAsync(quoteId, flightItemId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{flightItemId:guid}/segments/{segmentId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> UpdateSegment([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, [FromRoute] Guid segmentId, [FromBody] UpdateFlightSegmentRequest request, CancellationToken cancellationToken)
    {
        var result = await updateSegmentUseCase.ExecuteAsync(quoteId, flightItemId, segmentId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{flightItemId:guid}/segments/{segmentId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> DeleteSegment([FromRoute] Guid quoteId, [FromRoute] Guid flightItemId, [FromRoute] Guid segmentId, CancellationToken cancellationToken)
    {
        var result = await deleteSegmentUseCase.ExecuteAsync(quoteId, flightItemId, segmentId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
