using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Subscriptions;

public class PlanFeatureRepository(TripflowDbContext context) : BaseRepository<PlanFeature>(context), IPlanFeatureRepository
{
    public async Task<List<PlanFeature>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await _context.PlanFeatures
            .AsNoTracking()
            .Where(x => x.SubscriptionPlanId == planId)
            .OrderBy(x => x.FeatureCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlanFeature?> GetByPlanAndCodeAsync(Guid planId, string featureCode, CancellationToken cancellationToken = default)
    {
        return await _context.PlanFeatures
            .FirstOrDefaultAsync(x => x.SubscriptionPlanId == planId && x.FeatureCode == featureCode, cancellationToken);
    }
}
