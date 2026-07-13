using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Proposals;

public class ProposalEventRepository(TripflowDbContext context) : BaseRepository<ProposalEvent>(context), IProposalEventRepository
{
    public async Task<List<ProposalEvent>> GetByProposalIdAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.ProposalEvents
            .AsNoTracking()
            .Where(x => x.ProposalId == proposalId && x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
