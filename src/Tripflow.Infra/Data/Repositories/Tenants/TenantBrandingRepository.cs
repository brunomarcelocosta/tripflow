using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Tenants;

public class TenantBrandingRepository(TripflowDbContext context) : BaseRepository<TenantBranding>(context), ITenantBrandingRepository
{
    public async Task<TenantBranding?> GetByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantBrandings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantBranding?> GetTrackedByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantBrandings
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }
}
