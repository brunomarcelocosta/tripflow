using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Payments.Interfaces;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/webhooks/payments")]
[AllowAnonymous]
public sealed class PaymentWebhooksController(
    IReceivePaymentWebhookUseCase receivePaymentWebhookUseCase) : ControllerBase
{
    [HttpPost("{providerCode}")]
    public async Task<IActionResult> Receive([FromRoute] string providerCode, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);

        var result = await receivePaymentWebhookUseCase.ExecuteAsync(providerCode, payload, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
