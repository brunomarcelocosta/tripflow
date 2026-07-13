using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Domain.Interfaces;

public interface ITenantRepository : IBaseRepository<Tenant>
{
    Task<Tenant?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

    Task<Tenant?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Tenant?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
