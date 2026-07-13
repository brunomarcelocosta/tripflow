using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Quotes;

public class QuoteFlightItemRepository(TripflowDbContext context) : BaseRepository<QuoteFlightItem>(context), IQuoteFlightItemRepository
{
    public async Task<List<QuoteFlightItem>> GetByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuoteFlightItems
            .AsNoTracking()
            .Include(x => x.Segments.OrderBy(s => s.Sequence))
            .Where(x => x.TenantId == tenantId && x.QuoteId == quoteId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuoteFlightItem?> GetByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuoteFlightItems
            .AsNoTracking()
            .Include(x => x.Segments.OrderBy(s => s.Sequence))
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<QuoteFlightItem?> GetTrackedByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuoteFlightItems
            .Include(x => x.Segments)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }
}
