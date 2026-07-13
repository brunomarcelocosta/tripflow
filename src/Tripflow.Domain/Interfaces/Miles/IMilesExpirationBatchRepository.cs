using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Domain.Interfaces;

public interface IMilesExpirationBatchRepository : IBaseRepository<MilesExpirationBatch>
{
    Task<List<MilesExpirationBatch>> GetByAccountIdAsync(Guid accountId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<MilesExpirationBatch>> GetPendingByAccountOrderedAsync(Guid accountId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<MilesExpirationBatch?> GetTrackedByIdAndAccountAsync(Guid id, Guid accountId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<int> GetExpiringAmountAsync(Guid accountId, Guid tenantId, DateOnly untilDate, CancellationToken cancellationToken = default);
}
