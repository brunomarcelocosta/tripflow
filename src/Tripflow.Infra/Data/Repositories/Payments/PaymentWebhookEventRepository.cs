using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Payments;

public class PaymentWebhookEventRepository(TripflowDbContext context) : BaseRepository<PaymentWebhookEvent>(context), IPaymentWebhookEventRepository
{
    public async Task<bool> ExistsByProviderAndExternalEventIdAsync(string providerCode, string externalEventId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentWebhookEvents
            .AsNoTracking()
            .AnyAsync(x => x.ProviderCode == providerCode && x.ExternalEventId == externalEventId, cancellationToken);
    }

    public async Task<PaymentWebhookEvent?> GetUnprocessedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentWebhookEvents
            .FirstOrDefaultAsync(x => x.Id == id && !x.Processed, cancellationToken);
    }
}
