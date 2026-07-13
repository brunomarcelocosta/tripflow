using Tripflow.Domain.Entities;

namespace Tripflow.Domain.Interfaces;

public interface IItineraryStopRepository : IBaseRepository<ItineraryStop>
{
    Task<ItineraryStop?> GetByIdAndItineraryAsync(Guid stopId, Guid itineraryId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<ItineraryStop?> GetTrackedByIdAndItineraryAsync(Guid stopId, Guid itineraryId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<List<ItineraryStop>> GetByItineraryIdAsync(Guid itineraryId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<List<ItineraryStop>> GetTrackedByItineraryIdAsync(Guid itineraryId, Guid tenantId, CancellationToken cancellationToken = default);
}
