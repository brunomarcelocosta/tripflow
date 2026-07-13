using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Admin;

namespace Tripflow.Domain.Interfaces.Admin;

public interface ISupportSessionRepository : IBaseRepository<SupportSession>
{
    Task<SupportSession?> GetCurrentByAdminUserAsync(
        Guid adminUserProfileId,
        CancellationToken cancellationToken = default);

    Task<List<SupportSession>> GetByAdminUserAsync(
        Guid adminUserProfileId,
        CancellationToken cancellationToken = default);

    Task<SupportSession?> GetTrackedCurrentByAdminUserAsync(
        Guid adminUserProfileId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SupportSession>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
