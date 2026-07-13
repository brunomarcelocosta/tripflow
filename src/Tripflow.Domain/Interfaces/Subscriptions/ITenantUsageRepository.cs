using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Domain.Interfaces;

public interface ITenantUsageRepository : IBaseRepository<TenantUsage>
{
    Task<List<TenantUsage>> GetCurrentByTenantAsync(Guid tenantId, int year, int month, CancellationToken cancellationToken = default);
    Task<TenantUsage?> GetCurrentByTypeAsync(Guid tenantId, string usageType, int year, int month, CancellationToken cancellationToken = default);
    Task<TenantUsage?> GetTrackedCurrentByTypeAsync(Guid tenantId, string usageType, int year, int month, CancellationToken cancellationToken = default);
}
