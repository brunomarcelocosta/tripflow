using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Domain.Interfaces;

public interface IProposalVersionRepository : IBaseRepository<ProposalVersion>
{
    Task<List<ProposalVersion>> GetByProposalIdAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<ProposalVersion?> GetByIdAndProposalAsync(Guid id, Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<int> GetNextVersionNumberAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default);
}
