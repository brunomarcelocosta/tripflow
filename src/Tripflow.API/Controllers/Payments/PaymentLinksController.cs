using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/payments/{paymentId:guid}/links")]
[Authorize]
public sealed class PaymentLinksController(
    IGetPaymentLinksUseCase getPaymentLinksUseCase,
    ICreatePaymentLinkUseCase createPaymentLinkUseCase,
    ICancelPaymentLinkUseCase cancelPaymentLinkUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid paymentId, CancellationToken cancellationToken)
    {
        var result = await getPaymentLinksUseCase.ExecuteAsync(paymentId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsWrite)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid paymentId,
        [FromBody] CreatePaymentLinkRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createPaymentLinkUseCase.ExecuteAsync(paymentId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{linkId:guid}/cancel")]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsWrite)]
    public async Task<IActionResult> Cancel(
        [FromRoute] Guid paymentId,
        [FromRoute] Guid linkId,
        CancellationToken cancellationToken)
    {
        var result = await cancelPaymentLinkUseCase.ExecuteAsync(paymentId, linkId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
