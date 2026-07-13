using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Payments;

public class FinancialTransactionRepository(TripflowDbContext context) : BaseRepository<FinancialTransaction>(context), IFinancialTransactionRepository
{
    public async Task<FinancialTransaction?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.FinancialTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsSaleByPaymentIdAsync(Guid paymentId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.FinancialTransactions
            .AsNoTracking()
            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.PaymentId == paymentId &&
                x.Type == Domain.Enums.FinancialTransactionType.Sale,
                cancellationToken);
    }
}
