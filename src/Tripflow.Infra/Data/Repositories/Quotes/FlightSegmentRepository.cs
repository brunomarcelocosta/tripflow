using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Quotes;

public class FlightSegmentRepository(TripflowDbContext context) : BaseRepository<FlightSegment>(context), IFlightSegmentRepository
{
    public async Task<List<FlightSegment>> GetByFlightItemIdAsync(
        Guid quoteFlightItemId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.FlightSegments
            .AsNoTracking()
            .Where(x => x.QuoteFlightItemId == quoteFlightItemId && x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FlightSegment>> GetTrackedByFlightItemIdAsync(
        Guid quoteFlightItemId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.FlightSegments
            .Where(x => x.QuoteFlightItemId == quoteFlightItemId && x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<FlightSegment?> GetByIdAndFlightItemAsync(
        Guid segmentId,
        Guid quoteFlightItemId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.FlightSegments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == segmentId && x.QuoteFlightItemId == quoteFlightItemId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<FlightSegment?> GetTrackedByIdAndFlightItemAsync(
        Guid segmentId,
        Guid quoteFlightItemId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.FlightSegments
            .FirstOrDefaultAsync(
                x => x.Id == segmentId && x.QuoteFlightItemId == quoteFlightItemId && x.TenantId == tenantId,
                cancellationToken);
    }
}
