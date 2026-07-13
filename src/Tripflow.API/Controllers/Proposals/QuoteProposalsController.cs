using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Proposals;

[ApiController]
[Route("api/quotes")]
[Authorize]
public sealed class QuoteProposalsController(
    ICreateProposalFromQuoteUseCase createProposalFromQuoteUseCase) : ControllerBase
{
    [HttpPost("{quoteId:guid}/proposals")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> CreateFromQuote(
        [FromRoute] Guid quoteId,
        [FromBody] CreateProposalFromQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createProposalFromQuoteUseCase.ExecuteAsync(quoteId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
