using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Platform;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Platform;

public class PlatformPaymentEventRepository(TripflowDbContext context)
    : BaseRepository<PlatformPaymentEvent>(context), IPlatformPaymentEventRepository
{
    public async Task<bool> ExistsByProviderAndExternalEventIdAsync(string providerCode, string externalEventId, CancellationToken cancellationToken = default)
    {
        return await _context.PlatformPaymentEvents
            .AsNoTracking()
            .AnyAsync(x => x.ProviderCode == providerCode && x.ExternalEventId == externalEventId, cancellationToken);
    }
}
