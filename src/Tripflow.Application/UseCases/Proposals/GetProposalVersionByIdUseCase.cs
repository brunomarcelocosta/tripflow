using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class GetProposalVersionByIdUseCase(
    IProposalRepository proposalRepository,
    IProposalVersionRepository versionRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetProposalVersionByIdUseCase
{
    public async Task<Result<ProposalVersionResponse?>> ExecuteAsync(Guid proposalId, Guid versionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ProposalVersionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsRead, cancellationToken))
            return Result<ProposalVersionResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var proposal = await proposalRepository.GetByIdAndTenantAsync(proposalId, tenantId, cancellationToken);
        if (proposal is null)
            return Result<ProposalVersionResponse?>.Failure("Proposta não encontrada.");

        var version = await versionRepository.GetByIdAndProposalAsync(versionId, proposalId, tenantId, cancellationToken);
        if (version is null)
            return Result<ProposalVersionResponse?>.Failure("Versão não encontrada.");

        return Result<ProposalVersionResponse?>.Ok(mapper.Map<ProposalVersionResponse>(version));
    }
}
