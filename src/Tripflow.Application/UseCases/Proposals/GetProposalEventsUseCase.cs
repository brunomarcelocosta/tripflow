using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class GetProposalEventsUseCase(
    IProposalRepository proposalRepository,
    IProposalEventRepository eventRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetProposalEventsUseCase
{
    public async Task<Result<IEnumerable<ProposalEventResponse>>> ExecuteAsync(Guid proposalId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<ProposalEventResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsRead, cancellationToken))
            return Result<IEnumerable<ProposalEventResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var proposal = await proposalRepository.GetByIdAndTenantAsync(proposalId, tenantId, cancellationToken);
        if (proposal is null)
            return Result<IEnumerable<ProposalEventResponse>>.Failure("Proposta não encontrada.");

        var events = await eventRepository.GetByProposalIdAsync(proposalId, tenantId, cancellationToken);
        return Result<IEnumerable<ProposalEventResponse>>.Ok(events.Select(mapper.Map<ProposalEventResponse>));
    }
}
