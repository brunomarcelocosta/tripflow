using Tripflow.Domain.Entities.Platform;

namespace Tripflow.Domain.Interfaces;

public interface IPlatformCheckoutSessionRepository : IBaseRepository<PlatformCheckoutSession>
{
    Task<PlatformCheckoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PlatformCheckoutSession?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PlatformCheckoutSession?> GetByExternalCheckoutIdAsync(string externalCheckoutId, CancellationToken cancellationToken = default);

    Task<PlatformCheckoutSession?> GetTrackedByExternalCheckoutIdAsync(string externalCheckoutId, CancellationToken cancellationToken = default);

    Task<List<PlatformCheckoutSession>> GetByLeadIdAsync(Guid leadId, CancellationToken cancellationToken = default);
}
