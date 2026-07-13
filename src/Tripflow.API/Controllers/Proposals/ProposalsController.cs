using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Proposals;

[ApiController]
[Route("api/proposals")]
[Authorize]
public sealed class ProposalsController(
    IGetProposalsUseCase getProposalsUseCase,
    IGetProposalByIdUseCase getProposalByIdUseCase,
    IUpdateProposalUseCase updateProposalUseCase,
    IDeleteProposalUseCase deleteProposalUseCase,
    ICancelProposalUseCase cancelProposalUseCase,
    IMarkProposalAsSentUseCase markProposalAsSentUseCase,
    IExpireProposalUseCase expireProposalUseCase,
    IRegenerateProposalPublicLinkUseCase regenerateProposalPublicLinkUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsRead)]
    public async Task<IActionResult> GetAll([FromQuery] ProposalFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getProposalsUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getProposalByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProposalRequest request, CancellationToken cancellationToken)
    {
        var result = await updateProposalUseCase.ExecuteAsync(id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteProposalUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await cancelProposalUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/mark-as-sent")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsSend)]
    public async Task<IActionResult> MarkAsSent([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await markProposalAsSentUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/expire")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> Expire([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await expireProposalUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/public-link/regenerate")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> RegeneratePublicLink([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await regenerateProposalPublicLinkUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
