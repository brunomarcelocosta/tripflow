using Tripflow.Domain.Entities.Identity;

namespace Tripflow.Domain.Interfaces;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<List<Permission>> GetByCodesAsync(
        IReadOnlyList<string> codes,
        CancellationToken cancellationToken = default);

    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
}
