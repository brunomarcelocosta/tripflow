using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Pricing;

public class PaymentFeeRuleRepository(TripflowDbContext context) : BaseRepository<PaymentFeeRule>(context), IPaymentFeeRuleRepository
{
    public async Task<List<PaymentFeeRule>> GetByTenantIdAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentFeeRules
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.PaymentMethod)
            .ThenBy(x => x.Installments)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentFeeRule?> GetByTenantPaymentMethodAndInstallmentsAsync(
        Guid tenantId,
        PaymentMethod paymentMethod,
        int installments,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentFeeRules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId
                  && x.PaymentMethod == paymentMethod
                  && x.Installments == installments,
                cancellationToken);
    }

    public async Task<PaymentFeeRule?> GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
        Guid tenantId,
        PaymentMethod paymentMethod,
        int installments,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentFeeRules
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId
                  && x.PaymentMethod == paymentMethod
                  && x.Installments == installments,
                cancellationToken);
    }
}
