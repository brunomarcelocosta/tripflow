using Tripflow.Domain.Entities.Identity;

namespace Tripflow.Domain.Interfaces;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default);

    Task<List<Role>> GetByIdsAsync(
        Guid tenantId,
        IReadOnlyList<Guid> roleIds,
        CancellationToken cancellationToken = default);

    Task<List<Role>> GetByNamesAsync(
        Guid tenantId,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default);

    Task<List<Role>> GetAllByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
