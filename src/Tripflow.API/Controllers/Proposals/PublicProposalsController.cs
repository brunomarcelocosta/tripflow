using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;

namespace Tripflow.API.Controllers.Proposals;

[ApiController]
[Route("api/public/proposals")]
[AllowAnonymous]
public sealed class PublicProposalsController(
    IGetPublicProposalByTokenUseCase getPublicProposalByTokenUseCase,
    IApprovePublicProposalUseCase approvePublicProposalUseCase,
    IRejectPublicProposalUseCase rejectPublicProposalUseCase) : ControllerBase
{
    [HttpGet("{publicToken}")]
    public async Task<IActionResult> GetByToken([FromRoute] string publicToken, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await getPublicProposalByTokenUseCase.ExecuteAsync(publicToken, ip, userAgent, cancellationToken);
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{publicToken}/approve")]
    public async Task<IActionResult> Approve([FromRoute] string publicToken, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await approvePublicProposalUseCase.ExecuteAsync(publicToken, ip, userAgent, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{publicToken}/reject")]
    public async Task<IActionResult> Reject(
        [FromRoute] string publicToken,
        [FromBody] RejectPublicProposalRequest request,
        CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await rejectPublicProposalUseCase.ExecuteAsync(publicToken, request, ip, userAgent, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
