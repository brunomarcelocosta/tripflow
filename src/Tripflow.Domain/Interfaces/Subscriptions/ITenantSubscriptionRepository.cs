using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Domain.Interfaces;

public interface ITenantSubscriptionRepository : IBaseRepository<TenantSubscription>
{
    Task<TenantSubscription?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<TenantSubscription?> GetByTenantIdWithPlanFeaturesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<TenantSubscription?> GetTrackedByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
