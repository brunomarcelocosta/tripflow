using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Proposals;

[ApiController]
[Route("api/proposals/{proposalId:guid}/versions")]
[Authorize]
public sealed class ProposalVersionsController(
    IGetProposalVersionsUseCase getProposalVersionsUseCase,
    IGetProposalVersionByIdUseCase getProposalVersionByIdUseCase,
    IGenerateProposalVersionUseCase generateProposalVersionUseCase,
    IGenerateProposalPdfUseCase generateProposalPdfUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid proposalId, CancellationToken cancellationToken)
    {
        var result = await getProposalVersionsUseCase.ExecuteAsync(proposalId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{versionId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid proposalId, [FromRoute] Guid versionId, CancellationToken cancellationToken)
    {
        var result = await getProposalVersionByIdUseCase.ExecuteAsync(proposalId, versionId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> GenerateVersion([FromRoute] Guid proposalId, [FromBody] GenerateProposalVersionRequest request, CancellationToken cancellationToken)
    {
        var result = await generateProposalVersionUseCase.ExecuteAsync(proposalId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("~/api/proposals/{proposalId:guid}/generate-pdf")]
    [RequirePermission(TripflowDbSeedData.Permissions.ProposalsWrite)]
    public async Task<IActionResult> GeneratePdf([FromRoute] Guid proposalId, CancellationToken cancellationToken)
    {
        var result = await generateProposalPdfUseCase.ExecuteAsync(proposalId, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }
}
