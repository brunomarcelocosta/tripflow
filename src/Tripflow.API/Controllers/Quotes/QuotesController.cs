using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes")]
[Authorize]
public sealed class QuotesController(
    IGetQuotesUseCase getQuotesUseCase,
    IGetQuoteByIdUseCase getQuoteByIdUseCase,
    ICreateQuoteUseCase createQuoteUseCase,
    IUpdateQuoteUseCase updateQuoteUseCase,
    IDeleteQuoteUseCase deleteQuoteUseCase,
    ICancelQuoteUseCase cancelQuoteUseCase,
    IApproveQuoteUseCase approveQuoteUseCase,
    IRejectQuoteUseCase rejectQuoteUseCase,
    IMarkQuoteAsCalculatedUseCase markQuoteAsCalculatedUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetAll([FromQuery] QuoteFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getQuotesUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getQuoteByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Create([FromBody] CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        var result = await createQuoteUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateQuoteRequest request, CancellationToken cancellationToken)
    {
        var result = await updateQuoteUseCase.ExecuteAsync(id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteQuoteUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await cancelQuoteUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/approve")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesApprove)]
    public async Task<IActionResult> Approve([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await approveQuoteUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/reject")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesApprove)]
    public async Task<IActionResult> Reject([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await rejectQuoteUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/mark-as-calculated")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> MarkAsCalculated([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await markQuoteAsCalculatedUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
