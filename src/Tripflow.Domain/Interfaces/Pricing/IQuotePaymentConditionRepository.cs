using Tripflow.Domain.Entities.Pricing;

namespace Tripflow.Domain.Interfaces;

public interface IQuotePaymentConditionRepository : IBaseRepository<QuotePaymentCondition>
{
    Task<List<QuotePaymentCondition>> GetByPricingOptionIdAsync(Guid pricingOptionId, Guid tenantId, CancellationToken cancellationToken = default);

    Task DeleteByPricingOptionIdAsync(Guid pricingOptionId, Guid tenantId, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<QuotePaymentCondition> conditions, CancellationToken cancellationToken = default);
}
