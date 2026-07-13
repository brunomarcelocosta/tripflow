using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Payments;

public class PaymentRepository(TripflowDbContext context) : BaseRepository<Payment>(context), IPaymentRepository
{
    public async Task<Payment?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .AsNoTracking()
            .Include(x => x.Links)
            .Include(x => x.Proposal)
            .Include(x => x.TenantPaymentProvider!)
                .ThenInclude(p => p.PaymentProvider)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Payment?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Payment?> GetByExternalPaymentIdAsync(string externalPaymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.ExternalPaymentId == externalPaymentId, cancellationToken);
    }

    public async Task<Payment?> GetTrackedByExternalPaymentIdAsync(string externalPaymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.ExternalPaymentId == externalPaymentId, cancellationToken);
    }

    public async Task<Payment?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, cancellationToken);
    }
}
