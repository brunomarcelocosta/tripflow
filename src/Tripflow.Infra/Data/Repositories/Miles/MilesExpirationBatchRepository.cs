using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Miles;

public class MilesExpirationBatchRepository(TripflowDbContext context) : BaseRepository<MilesExpirationBatch>(context), IMilesExpirationBatchRepository
{
    public async Task<List<MilesExpirationBatch>> GetByAccountIdAsync(Guid accountId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MilesExpirationBatches
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerLoyaltyAccountId == accountId)
            .OrderBy(x => x.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MilesExpirationBatch>> GetPendingByAccountOrderedAsync(Guid accountId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MilesExpirationBatches
            .Where(x => x.TenantId == tenantId && x.CustomerLoyaltyAccountId == accountId && x.Status == MilesExpirationStatus.Pending)
            .OrderBy(x => x.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MilesExpirationBatch?> GetTrackedByIdAndAccountAsync(Guid id, Guid accountId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MilesExpirationBatches
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerLoyaltyAccountId == accountId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<int> GetExpiringAmountAsync(Guid accountId, Guid tenantId, DateOnly untilDate, CancellationToken cancellationToken = default)
    {
        return await _context.MilesExpirationBatches
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId
                && x.CustomerLoyaltyAccountId == accountId
                && x.Status == MilesExpirationStatus.Pending
                && x.ExpiresAt <= untilDate)
            .SumAsync(x => x.RemainingAmount, cancellationToken);
    }
}
