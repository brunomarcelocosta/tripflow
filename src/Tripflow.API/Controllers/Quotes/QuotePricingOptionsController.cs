using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes/{quoteId:guid}/pricing-options")]
[Authorize]
public sealed class QuotePricingOptionsController(
    IGetQuotePricingOptionsUseCase getOptionsUseCase,
    IGetQuotePricingOptionByIdUseCase getOptionByIdUseCase,
    ICreateQuotePricingOptionUseCase createOptionUseCase,
    IUpdateQuotePricingOptionUseCase updateOptionUseCase,
    IDeleteQuotePricingOptionUseCase deleteOptionUseCase,
    ISelectQuotePricingOptionUseCase selectOptionUseCase) : ControllerBase
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

    [HttpGet("{pricingOptionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid quoteId, [FromRoute] Guid pricingOptionId, CancellationToken cancellationToken)
    {
        var result = await getOptionByIdUseCase.ExecuteAsync(quoteId, pricingOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid quoteId, [FromBody] CreateQuotePricingOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await createOptionUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{pricingOptionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid quoteId, [FromRoute] Guid pricingOptionId, [FromBody] UpdateQuotePricingOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await updateOptionUseCase.ExecuteAsync(quoteId, pricingOptionId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{pricingOptionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid quoteId, [FromRoute] Guid pricingOptionId, CancellationToken cancellationToken)
    {
        var result = await deleteOptionUseCase.ExecuteAsync(quoteId, pricingOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{pricingOptionId:guid}/select")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Select([FromRoute] Guid quoteId, [FromRoute] Guid pricingOptionId, CancellationToken cancellationToken)
    {
        var result = await selectOptionUseCase.ExecuteAsync(quoteId, pricingOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
