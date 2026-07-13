using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Proposals;

public class ProposalVersionRepository(TripflowDbContext context) : BaseRepository<ProposalVersion>(context), IProposalVersionRepository
{
    public async Task<List<ProposalVersion>> GetByProposalIdAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.ProposalVersions
            .AsNoTracking()
            .Where(x => x.ProposalId == proposalId && x.TenantId == tenantId)
            .OrderByDescending(x => x.VersionNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProposalVersion?> GetByIdAndProposalAsync(Guid id, Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.ProposalVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.ProposalId == proposalId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<int> GetNextVersionNumberAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var max = await _context.ProposalVersions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.ProposalId == proposalId && x.TenantId == tenantId)
            .MaxAsync(x => (int?)x.VersionNumber, cancellationToken);

        return (max ?? 0) + 1;
    }
}
