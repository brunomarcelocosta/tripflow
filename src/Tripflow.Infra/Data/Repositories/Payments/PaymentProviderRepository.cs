using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Payments;

public class PaymentProviderRepository(TripflowDbContext context) : BaseRepository<PaymentProvider>(context), IPaymentProviderRepository
{
    public async Task<List<PaymentProvider>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProviders
            .AsNoTracking()
            .Where(x => x.Status == PaymentProviderStatus.Active)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentProvider?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProviders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }
}
