using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Platform.Interfaces;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/webhooks/infinitepay")]
[AllowAnonymous]
public sealed class InfinitePayPlatformWebhookController(
    IProcessPlatformCheckoutWebhookUseCase processPlatformCheckoutWebhookUseCase) : ControllerBase
{
    [HttpPost("platform")]
    public async Task<IActionResult> ReceivePlatformCheckout(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);

        var headers = Request.Headers
            .ToDictionary(
                header => header.Key,
                header => header.Value.ToString(),
                StringComparer.OrdinalIgnoreCase);

        var result = await processPlatformCheckoutWebhookUseCase.ExecuteAsync(payload, headers, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
