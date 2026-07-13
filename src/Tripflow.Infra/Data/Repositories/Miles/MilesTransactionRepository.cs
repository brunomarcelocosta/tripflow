using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Miles;

public class MilesTransactionRepository(TripflowDbContext context) : BaseRepository<MilesTransaction>(context), IMilesTransactionRepository
{
    public async Task<List<MilesTransaction>> GetByAccountIdAsync(Guid accountId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MilesTransactions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.CustomerLoyaltyAccountId == accountId)
            .OrderByDescending(x => x.TransactionDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<MilesTransaction?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MilesTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<MilesTransaction?> GetByIdAndAccountAsync(Guid id, Guid accountId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.MilesTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerLoyaltyAccountId == accountId && x.TenantId == tenantId, cancellationToken);
    }
}
