using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Domain.Interfaces;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<Payment?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<Payment?> GetByExternalPaymentIdAsync(string externalPaymentId, CancellationToken cancellationToken = default);

    Task<Payment?> GetTrackedByExternalPaymentIdAsync(string externalPaymentId, CancellationToken cancellationToken = default);

    Task<Payment?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
