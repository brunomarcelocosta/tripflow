using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Domain.Interfaces;

public interface IProposalRepository : IBaseRepository<Proposal>
{
    Task<Proposal?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<Proposal?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<Proposal?> GetByPublicTokenAsync(string publicToken, CancellationToken cancellationToken = default);

    Task<Proposal?> GetTrackedByPublicTokenAsync(string publicToken, CancellationToken cancellationToken = default);

    Task<Proposal?> GetFullByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByProposalNumberAsync(Guid tenantId, string proposalNumber, CancellationToken cancellationToken = default);

    Task<string> GenerateNextProposalNumberAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, (int VersionsCount, int EventsCount)>> GetAggregatesAsync(
        Guid tenantId,
        IEnumerable<Guid> proposalIds,
        CancellationToken cancellationToken = default);
}
