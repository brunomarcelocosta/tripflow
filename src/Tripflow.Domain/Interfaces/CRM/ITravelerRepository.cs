using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Domain.Interfaces;

public interface ITravelerRepository : IBaseRepository<Traveler>
{
    Task<Traveler?> GetByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Traveler?> GetByCustomerAndTenantAsync(
        Guid travelerId,
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Traveler?> GetTrackedByCustomerAndTenantAsync(
        Guid travelerId,
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsPassportNumberAsync(
        Guid tenantId,
        string passportNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsPassportNumberExceptIdAsync(
        Guid tenantId,
        string passportNumber,
        Guid travelerId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> CountByCustomersAsync(
        Guid tenantId,
        IEnumerable<Guid> customerIds,
        CancellationToken cancellationToken = default);
}
