using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Domain.Interfaces;

public interface IProposalEventRepository : IBaseRepository<ProposalEvent>
{
    Task<List<ProposalEvent>> GetByProposalIdAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default);
}
