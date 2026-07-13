using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Tenants;

public class TenantCommercialSettingsRepository(TripflowDbContext context) : BaseRepository<TenantCommercialSettings>(context), ITenantCommercialSettingsRepository
{
    public async Task<TenantCommercialSettings?> GetByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantCommercialSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantCommercialSettings?> GetTrackedByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantCommercialSettings
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }
}
