using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Domain.Interfaces;

public interface IPaymentWebhookEventRepository : IBaseRepository<PaymentWebhookEvent>
{
    Task<bool> ExistsByProviderAndExternalEventIdAsync(string providerCode, string externalEventId, CancellationToken cancellationToken = default);

    Task<PaymentWebhookEvent?> GetUnprocessedAsync(Guid id, CancellationToken cancellationToken = default);
}
