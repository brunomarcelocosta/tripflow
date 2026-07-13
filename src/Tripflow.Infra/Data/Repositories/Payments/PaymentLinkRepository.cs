using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Payments;

public class PaymentLinkRepository(TripflowDbContext context) : BaseRepository<PaymentLink>(context), IPaymentLinkRepository
{
    public async Task<List<PaymentLink>> GetByPaymentIdAsync(Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentLinks
            .AsNoTracking()
            .Where(x => x.PaymentId == paymentId && x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentLink?> GetTrackedByIdAndPaymentAsync(Guid id, Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentLinks
            .FirstOrDefaultAsync(x => x.Id == id && x.PaymentId == paymentId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<PaymentLink?> GetActiveByPaymentIdAsync(Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentLinks
            .AsNoTracking()
            .Where(x => x.PaymentId == paymentId && x.TenantId == tenantId && x.Status == Domain.Enums.PaymentLinkStatus.Active)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaymentLink?> GetTrackedActiveByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentLinks
            .IgnoreQueryFilters()
            .Where(x => !x.IsDeleted && x.PaymentId == paymentId && x.Status == Domain.Enums.PaymentLinkStatus.Active)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
