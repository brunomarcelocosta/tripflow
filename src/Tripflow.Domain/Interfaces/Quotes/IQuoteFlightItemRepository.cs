using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Domain.Interfaces;

public interface IQuoteFlightItemRepository : IBaseRepository<QuoteFlightItem>
{
    Task<List<QuoteFlightItem>> GetByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<QuoteFlightItem?> GetByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<QuoteFlightItem?> GetTrackedByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);
}
