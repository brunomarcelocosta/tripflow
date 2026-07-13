using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Quotes;

public class ItineraryStopRepository(TripflowDbContext context) : BaseRepository<ItineraryStop>(context), IItineraryStopRepository
{
    public async Task<ItineraryStop?> GetByIdAndItineraryAsync(
        Guid stopId,
        Guid itineraryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ItineraryStops
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == stopId && x.ItineraryId == itineraryId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<ItineraryStop?> GetTrackedByIdAndItineraryAsync(
        Guid stopId,
        Guid itineraryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ItineraryStops
            .FirstOrDefaultAsync(
                x => x.Id == stopId && x.ItineraryId == itineraryId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<List<ItineraryStop>> GetByItineraryIdAsync(
        Guid itineraryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ItineraryStops
            .AsNoTracking()
            .Where(x => x.ItineraryId == itineraryId && x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ItineraryStop>> GetTrackedByItineraryIdAsync(
        Guid itineraryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ItineraryStops
            .Where(x => x.ItineraryId == itineraryId && x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }
}
