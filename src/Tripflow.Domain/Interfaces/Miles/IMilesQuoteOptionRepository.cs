using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Domain.Interfaces;

public interface IMilesQuoteOptionRepository : IBaseRepository<MilesQuoteOption>
{
    Task<List<MilesQuoteOption>> GetByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<MilesQuoteOption?> GetByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<MilesQuoteOption?> GetTrackedByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<List<MilesQuoteOption>> GetTrackedByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);
}
