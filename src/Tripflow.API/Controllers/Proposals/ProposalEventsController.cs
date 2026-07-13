using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Proposals;

[ApiController]
[Route("api/proposals/{proposalId:guid}/events")]
[Authorize]
public sealed class ProposalEventsController(IGetProposalEventsUseCase getProposalEventsUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid proposalId, CancellationToken cancellationToken)
    {
        var result = await getProposalEventsUseCase.ExecuteAsync(proposalId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
