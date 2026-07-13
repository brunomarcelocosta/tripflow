using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Domain.Interfaces;

public interface IQuoteRepository : IBaseRepository<Quote>
{
    Task<Quote?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<Quote?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByQuoteNumberAsync(Guid tenantId, string quoteNumber, CancellationToken cancellationToken = default);

    Task<string> GenerateNextQuoteNumberAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, (int ItemsCount, int FlightItemsCount, bool HasItinerary)>> GetAggregatesAsync(
        Guid tenantId,
        IEnumerable<Guid> quoteIds,
        CancellationToken cancellationToken = default);
}
