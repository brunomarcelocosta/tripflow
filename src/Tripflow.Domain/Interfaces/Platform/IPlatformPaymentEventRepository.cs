using Tripflow.Domain.Entities.Platform;

namespace Tripflow.Domain.Interfaces;

public interface IPlatformPaymentEventRepository : IBaseRepository<PlatformPaymentEvent>
{
    Task<bool> ExistsByProviderAndExternalEventIdAsync(string providerCode, string externalEventId, CancellationToken cancellationToken = default);
}
