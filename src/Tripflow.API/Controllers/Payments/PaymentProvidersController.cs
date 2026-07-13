using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Payments.Interfaces;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/payment-providers")]
[Authorize]
public sealed class PaymentProvidersController(
    IGetPaymentProvidersUseCase getPaymentProvidersUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await getPaymentProvidersUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
