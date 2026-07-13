using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Domain.Interfaces;

public interface ISubscriptionPlanRepository : IBaseRepository<SubscriptionPlan>
{
    Task<SubscriptionPlan?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<SubscriptionPlan?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SubscriptionPlan?> GetByIdWithFeaturesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameExceptIdAsync(string name, Guid id, CancellationToken cancellationToken = default);
    Task<List<SubscriptionPlan>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<SubscriptionPlan>> GetActiveWithFeaturesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveSubscriptionsAsync(Guid planId, CancellationToken cancellationToken = default);
}
