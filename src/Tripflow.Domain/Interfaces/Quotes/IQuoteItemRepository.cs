using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Domain.Interfaces;

public interface IQuoteItemRepository : IBaseRepository<QuoteItem>
{
    Task<List<QuoteItem>> GetByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<QuoteItem?> GetByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<QuoteItem?> GetTrackedByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);
}
