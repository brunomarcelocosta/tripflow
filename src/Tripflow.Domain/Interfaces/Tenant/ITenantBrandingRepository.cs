using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Domain.Interfaces;

public interface ITenantBrandingRepository : IBaseRepository<TenantBranding>
{
    Task<TenantBranding?> GetByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<TenantBranding?> GetTrackedByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
