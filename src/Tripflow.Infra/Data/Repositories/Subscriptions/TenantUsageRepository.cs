using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Subscriptions;

public class TenantUsageRepository(TripflowDbContext context) : BaseRepository<TenantUsage>(context), ITenantUsageRepository
{
    public async Task<List<TenantUsage>> GetCurrentByTenantAsync(Guid tenantId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.TenantUsages
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.PeriodYear == year && x.PeriodMonth == month)
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantUsage?> GetCurrentByTypeAsync(Guid tenantId, string usageType, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.TenantUsages
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId
                && x.UsageType == usageType
                && x.PeriodYear == year
                && x.PeriodMonth == month,
                cancellationToken);
    }

    public async Task<TenantUsage?> GetTrackedCurrentByTypeAsync(Guid tenantId, string usageType, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.TenantUsages
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId
                && x.UsageType == usageType
                && x.PeriodYear == year
                && x.PeriodMonth == month,
                cancellationToken);
    }
}
