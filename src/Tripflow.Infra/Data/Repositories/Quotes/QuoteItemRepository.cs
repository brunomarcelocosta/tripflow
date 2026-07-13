using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Quotes;

public class QuoteItemRepository(TripflowDbContext context) : BaseRepository<QuoteItem>(context), IQuoteItemRepository
{
    public async Task<List<QuoteItem>> GetByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuoteItems
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.QuoteId == quoteId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuoteItem?> GetByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuoteItems
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<QuoteItem?> GetTrackedByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuoteItems
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }
}
