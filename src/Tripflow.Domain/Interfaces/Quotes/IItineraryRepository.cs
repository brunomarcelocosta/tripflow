using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Domain.Interfaces;

public interface IItineraryRepository : IBaseRepository<Itinerary>
{
    Task<Itinerary?> GetByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<Itinerary?> GetTrackedByQuoteIdAsync(Guid quoteId, Guid tenantId, CancellationToken cancellationToken = default);
}
