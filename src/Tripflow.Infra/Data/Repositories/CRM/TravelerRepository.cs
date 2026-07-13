using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.CRM;

public class TravelerRepository(TripflowDbContext context) : BaseRepository<Traveler>(context), ITravelerRepository
{
    public async Task<Traveler?> GetByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Travelers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Traveler?> GetByCustomerAndTenantAsync(
        Guid travelerId,
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Travelers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == travelerId
                  && x.CustomerId == customerId
                  && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<Traveler?> GetTrackedByCustomerAndTenantAsync(
        Guid travelerId,
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Travelers
            .FirstOrDefaultAsync(
                x => x.Id == travelerId
                  && x.CustomerId == customerId
                  && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<bool> ExistsPassportNumberAsync(
        Guid tenantId,
        string passportNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Travelers
            .AsNoTracking()
            .AnyAsync(x => x.TenantId == tenantId && x.PassportNumber == passportNumber, cancellationToken);
    }

    public async Task<bool> ExistsPassportNumberExceptIdAsync(
        Guid tenantId,
        string passportNumber,
        Guid travelerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Travelers
            .AsNoTracking()
            .AnyAsync(
                x => x.TenantId == tenantId
                  && x.PassportNumber == passportNumber
                  && x.Id != travelerId,
                cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> CountByCustomersAsync(
        Guid tenantId,
        IEnumerable<Guid> customerIds,
        CancellationToken cancellationToken = default)
    {
        var ids = customerIds.Distinct().ToList();

        if (ids.Count == 0)
            return new Dictionary<Guid, int>();

        var counts = await _context.Travelers
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && ids.Contains(x.CustomerId))
            .GroupBy(x => x.CustomerId)
            .Select(g => new { CustomerId = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(x => x.CustomerId, x => x.Total);
    }
}
