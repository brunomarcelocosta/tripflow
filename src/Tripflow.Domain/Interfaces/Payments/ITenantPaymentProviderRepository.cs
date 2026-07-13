using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Domain.Interfaces;

public interface ITenantPaymentProviderRepository : IBaseRepository<TenantPaymentProvider>
{
    Task<List<TenantPaymentProvider>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantPaymentProvider?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantPaymentProvider?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<TenantPaymentProvider?> GetDefaultByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<List<TenantPaymentProvider>> GetTrackedByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
