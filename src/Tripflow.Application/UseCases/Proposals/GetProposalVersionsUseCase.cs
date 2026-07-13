using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class GetProposalVersionsUseCase(
    IProposalRepository proposalRepository,
    IProposalVersionRepository versionRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetProposalVersionsUseCase
{
    public async Task<Result<IEnumerable<ProposalVersionResponse>>> ExecuteAsync(Guid proposalId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<ProposalVersionResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsRead, cancellationToken))
            return Result<IEnumerable<ProposalVersionResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var proposal = await proposalRepository.GetByIdAndTenantAsync(proposalId, tenantId, cancellationToken);
        if (proposal is null)
            return Result<IEnumerable<ProposalVersionResponse>>.Failure("Proposta não encontrada.");

        var versions = await versionRepository.GetByProposalIdAsync(proposalId, tenantId, cancellationToken);
        return Result<IEnumerable<ProposalVersionResponse>>.Ok(versions.Select(mapper.Map<ProposalVersionResponse>));
    }
}
