using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Domain.Interfaces;

public interface IPaymentLinkRepository : IBaseRepository<PaymentLink>
{
    Task<List<PaymentLink>> GetByPaymentIdAsync(Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<PaymentLink?> GetTrackedByIdAndPaymentAsync(Guid id, Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<PaymentLink?> GetActiveByPaymentIdAsync(Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<PaymentLink?> GetTrackedActiveByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
}
