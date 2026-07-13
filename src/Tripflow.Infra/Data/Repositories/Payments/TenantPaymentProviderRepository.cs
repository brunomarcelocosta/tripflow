using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Payments;

public class TenantPaymentProviderRepository(TripflowDbContext context) : BaseRepository<TenantPaymentProvider>(context), ITenantPaymentProviderRepository
{
    public async Task<List<TenantPaymentProvider>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantPaymentProviders
            .AsNoTracking()
            .Include(x => x.PaymentProvider)
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantPaymentProvider?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantPaymentProviders
            .AsNoTracking()
            .Include(x => x.PaymentProvider)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantPaymentProvider?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantPaymentProviders
            .Include(x => x.PaymentProvider)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantPaymentProvider?> GetDefaultByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantPaymentProviders
            .AsNoTracking()
            .Include(x => x.PaymentProvider)
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsDefault, cancellationToken);
    }

    public async Task<List<TenantPaymentProvider>> GetTrackedByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantPaymentProviders
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }
}
