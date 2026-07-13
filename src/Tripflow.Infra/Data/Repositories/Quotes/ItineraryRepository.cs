using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Quotes;

public class ItineraryRepository(TripflowDbContext context) : BaseRepository<Itinerary>(context), IItineraryRepository
{
    public async Task<Itinerary?> GetByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Itineraries
            .AsNoTracking()
            .Include(x => x.Stops)
            .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Itinerary?> GetTrackedByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Itineraries
            .Include(x => x.Stops)
            .FirstOrDefaultAsync(x => x.QuoteId == quoteId && x.TenantId == tenantId, cancellationToken);
    }
}
