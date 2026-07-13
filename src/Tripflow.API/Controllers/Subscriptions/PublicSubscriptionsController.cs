using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.UseCases.Platform.Interfaces;

namespace Tripflow.API.Controllers.Subscriptions;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
public sealed class PublicSubscriptionsController(
    IGetPublicSubscriptionPlansUseCase getPublicSubscriptionPlansUseCase,
    ICreatePlatformCheckoutUseCase createPlatformCheckoutUseCase,
    IGetPlatformCheckoutStatusUseCase getPlatformCheckoutStatusUseCase) : ControllerBase
{
    [HttpGet("subscription-plans")]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var result = await getPublicSubscriptionPlansUseCase.ExecuteAsync(cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout(
        [FromBody] CreatePlatformCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createPlatformCheckoutUseCase.ExecuteAsync(request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("checkout/{checkoutSessionId:guid}")]
    public async Task<IActionResult> GetCheckoutStatus(
        [FromRoute] Guid checkoutSessionId,
        CancellationToken cancellationToken)
    {
        var result = await getPlatformCheckoutStatusUseCase.ExecuteAsync(checkoutSessionId, cancellationToken);
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }
}
