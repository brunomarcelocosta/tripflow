using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Subscriptions;

public class SubscriptionPlanRepository(TripflowDbContext context) : BaseRepository<SubscriptionPlan>(context), ISubscriptionPlanRepository
{
    public async Task<SubscriptionPlan?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<SubscriptionPlan?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SubscriptionPlan?> GetByIdWithFeaturesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .Include(x => x.Features)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameExceptIdAsync(string name, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }

    public async Task<List<SubscriptionPlan>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(x => x.Status == SubscriptionPlanStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SubscriptionPlan>> GetActiveWithFeaturesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .Include(x => x.Features)
            .Where(x => x.Status == SubscriptionPlanStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveSubscriptionsAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantSubscriptions
            .AsNoTracking()
            .AnyAsync(x =>
                x.SubscriptionPlanId == planId
                && x.Status != TenantSubscriptionStatus.Cancelled,
                cancellationToken);
    }
}
