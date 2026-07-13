using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Subscriptions;

public class TenantSubscriptionRepository(TripflowDbContext context) : BaseRepository<TenantSubscription>(context), ITenantSubscriptionRepository
{
    public async Task<TenantSubscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantSubscriptions
            .AsNoTracking()
            .Include(x => x.SubscriptionPlan)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantSubscription?> GetByTenantIdWithPlanFeaturesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantSubscriptions
            .AsNoTracking()
            .Include(x => x.SubscriptionPlan)
                .ThenInclude(x => x.Features)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantSubscription?> GetTrackedByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantSubscriptions
            .Include(x => x.SubscriptionPlan)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
    }
}
