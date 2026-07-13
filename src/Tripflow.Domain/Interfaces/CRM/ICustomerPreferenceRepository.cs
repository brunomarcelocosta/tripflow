using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Domain.Interfaces;

public interface ICustomerPreferenceRepository : IBaseRepository<CustomerPreference>
{
    Task<CustomerPreference?> GetByCustomerAndTenantAsync(
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<CustomerPreference?> GetTrackedByCustomerAndTenantAsync(
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
