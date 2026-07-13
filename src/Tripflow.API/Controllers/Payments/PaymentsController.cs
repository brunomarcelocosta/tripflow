using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class PaymentsController(
    IGetPaymentsUseCase getPaymentsUseCase,
    IGetPaymentByIdUseCase getPaymentByIdUseCase,
    IMarkPaymentAsPaidUseCase markPaymentAsPaidUseCase,
    ICancelPaymentUseCase cancelPaymentUseCase,
    IRefundPaymentUseCase refundPaymentUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsRead)]
    public async Task<IActionResult> GetAll([FromQuery] PaymentFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getPaymentsUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getPaymentByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/mark-as-paid")]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsWrite)]
    public async Task<IActionResult> MarkAsPaid([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await markPaymentAsPaidUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsWrite)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await cancelPaymentUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/refund")]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsWrite)]
    public async Task<IActionResult> Refund([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await refundPaymentUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}

[ApiController]
[Route("api/proposals")]
[Authorize]
public sealed class ProposalPaymentsController(
    ICreatePaymentFromProposalUseCase createPaymentFromProposalUseCase) : ControllerBase
{
    [HttpPost("{proposalId:guid}/payments")]
    [RequirePermission(TripflowDbSeedData.Permissions.PaymentsWrite)]
    public async Task<IActionResult> CreateFromProposal(
        [FromRoute] Guid proposalId,
        [FromBody] CreatePaymentFromProposalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createPaymentFromProposalUseCase.ExecuteAsync(proposalId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
