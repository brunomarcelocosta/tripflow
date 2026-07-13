using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Miles;

public class MilesQuoteOptionRepository(TripflowDbContext context) : BaseRepository<MilesQuoteOption>(context), IMilesQuoteOptionRepository
{
    public async Task<List<MilesQuoteOption>> GetByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MilesQuoteOptions
            .AsNoTracking()
            .Include(x => x.LoyaltyProgram)
            .Where(x => x.TenantId == tenantId && x.QuoteId == quoteId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<MilesQuoteOption?> GetByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MilesQuoteOptions
            .AsNoTracking()
            .Include(x => x.LoyaltyProgram)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<MilesQuoteOption?> GetTrackedByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MilesQuoteOptions
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<List<MilesQuoteOption>> GetTrackedByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MilesQuoteOptions
            .Where(x => x.TenantId == tenantId && x.QuoteId == quoteId)
            .ToListAsync(cancellationToken);
    }
}
