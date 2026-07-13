using Tripflow.Domain.Entities.Pricing;

namespace Tripflow.Domain.Interfaces;

public interface IQuotePricingOptionRepository : IBaseRepository<QuotePricingOption>
{
    Task<List<QuotePricingOption>> GetByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<QuotePricingOption?> GetByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<QuotePricingOption?> GetTrackedByIdAndQuoteAsync(Guid id, Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<List<QuotePricingOption>> GetTrackedByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);
}
