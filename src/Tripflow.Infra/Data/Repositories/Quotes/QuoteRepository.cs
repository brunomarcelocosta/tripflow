using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Quotes;

public class QuoteRepository(TripflowDbContext context) : BaseRepository<Quote>(context), IQuoteRepository
{
    public async Task<Quote?> GetByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Quotes
            .AsNoTracking()
            .Include(q => q.Customer)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Quote?> GetTrackedByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Quotes
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsByQuoteNumberAsync(
        Guid tenantId,
        string quoteNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Quotes
            .AsNoTracking()
            .AnyAsync(x => x.TenantId == tenantId && x.QuoteNumber == quoteNumber, cancellationToken);
    }

    public async Task<string> GenerateNextQuoteNumberAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var prefix = $"TF-{DateTime.UtcNow:yyyyMM}-";

        var existingNumbers = await _context.Quotes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.QuoteNumber.StartsWith(prefix))
            .Select(x => x.QuoteNumber)
            .ToListAsync(cancellationToken);

        var maxSeq = 0;

        foreach (var number in existingNumbers)
        {
            var suffix = number[prefix.Length..];
            if (int.TryParse(suffix, out var parsed) && parsed > maxSeq)
                maxSeq = parsed;
        }

        var next = maxSeq + 1;
        return $"{prefix}{next:D6}";
    }

    public async Task<Dictionary<Guid, (int ItemsCount, int FlightItemsCount, bool HasItinerary)>> GetAggregatesAsync(
        Guid tenantId,
        IEnumerable<Guid> quoteIds,
        CancellationToken cancellationToken = default)
    {
        var ids = quoteIds.Distinct().ToList();

        if (ids.Count == 0)
            return new Dictionary<Guid, (int, int, bool)>();

        var items = await _context.QuoteItems
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && ids.Contains(x.QuoteId))
            .GroupBy(x => x.QuoteId)
            .Select(g => new { QuoteId = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        var flights = await _context.QuoteFlightItems
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && ids.Contains(x.QuoteId))
            .GroupBy(x => x.QuoteId)
            .Select(g => new { QuoteId = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        var itineraries = await _context.Itineraries
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && ids.Contains(x.QuoteId))
            .Select(x => x.QuoteId)
            .ToListAsync(cancellationToken);

        var itinerarySet = itineraries.ToHashSet();
        var itemsMap = items.ToDictionary(x => x.QuoteId, x => x.Total);
        var flightsMap = flights.ToDictionary(x => x.QuoteId, x => x.Total);

        var result = new Dictionary<Guid, (int, int, bool)>();

        foreach (var id in ids)
        {
            result[id] = (
                itemsMap.TryGetValue(id, out var i) ? i : 0,
                flightsMap.TryGetValue(id, out var f) ? f : 0,
                itinerarySet.Contains(id));
        }

        return result;
    }
}
