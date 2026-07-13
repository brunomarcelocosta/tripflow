using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Domain.Interfaces;

public interface ITenantCommercialSettingsRepository : IBaseRepository<TenantCommercialSettings>
{
    Task<TenantCommercialSettings?> GetByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<TenantCommercialSettings?> GetTrackedByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
