using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Domain.Interfaces;

public interface IMilesTransactionRepository : IBaseRepository<MilesTransaction>
{
    Task<List<MilesTransaction>> GetByAccountIdAsync(Guid accountId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<MilesTransaction?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<MilesTransaction?> GetByIdAndAccountAsync(Guid id, Guid accountId, Guid tenantId, CancellationToken cancellationToken = default);
}
