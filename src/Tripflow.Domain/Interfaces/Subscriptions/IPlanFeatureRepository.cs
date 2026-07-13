using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Domain.Interfaces;

public interface IPlanFeatureRepository : IBaseRepository<PlanFeature>
{
    Task<List<PlanFeature>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken = default);
    Task<PlanFeature?> GetByPlanAndCodeAsync(Guid planId, string featureCode, CancellationToken cancellationToken = default);
}
