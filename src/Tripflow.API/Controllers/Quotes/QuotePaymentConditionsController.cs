using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Quotes;

[ApiController]
[Route("api/quotes/{quoteId:guid}/pricing-options/{pricingOptionId:guid}/payment-conditions")]
[Authorize]
public sealed class QuotePaymentConditionsController(
    IGetQuotePaymentConditionsUseCase getConditionsUseCase,
    IRegenerateQuotePaymentConditionsUseCase regenerateConditionsUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesRead)]
    public async Task<IActionResult> Get([FromRoute] Guid quoteId, [FromRoute] Guid pricingOptionId, CancellationToken cancellationToken)
    {
        var result = await getConditionsUseCase.ExecuteAsync(quoteId, pricingOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("regenerate")]
    [RequirePermission(TripflowDbSeedData.Permissions.QuotesWrite)]
    public async Task<IActionResult> Regenerate([FromRoute] Guid quoteId, [FromRoute] Guid pricingOptionId, CancellationToken cancellationToken)
    {
        var result = await regenerateConditionsUseCase.ExecuteAsync(quoteId, pricingOptionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
