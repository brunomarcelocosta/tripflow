using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Domain.Interfaces;

public interface IFlightSegmentRepository : IBaseRepository<FlightSegment>
{
    Task<List<FlightSegment>> GetByFlightItemIdAsync(Guid quoteFlightItemId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<List<FlightSegment>> GetTrackedByFlightItemIdAsync(Guid quoteFlightItemId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<FlightSegment?> GetByIdAndFlightItemAsync(Guid segmentId, Guid quoteFlightItemId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<FlightSegment?> GetTrackedByIdAndFlightItemAsync(Guid segmentId, Guid quoteFlightItemId, Guid tenantId, CancellationToken cancellationToken = default);
}
