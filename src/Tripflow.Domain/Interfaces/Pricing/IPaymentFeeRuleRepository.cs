using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Interfaces;

public interface IPaymentFeeRuleRepository : IBaseRepository<PaymentFeeRule>
{
    Task<List<PaymentFeeRule>> GetByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<PaymentFeeRule?> GetByTenantPaymentMethodAndInstallmentsAsync(
        Guid tenantId,
        PaymentMethod paymentMethod,
        int installments,
        CancellationToken cancellationToken = default);

    Task<PaymentFeeRule?> GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
        Guid tenantId,
        PaymentMethod paymentMethod,
        int installments,
        CancellationToken cancellationToken = default);
}
