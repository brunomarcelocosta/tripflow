using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Domain.Interfaces;

public interface IFinancialTransactionRepository : IBaseRepository<FinancialTransaction>
{
    Task<FinancialTransaction?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> ExistsSaleByPaymentIdAsync(Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default);
}
