using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Subscriptions;

public class LeadRepository(TripflowDbContext context) : BaseRepository<Lead>(context), ILeadRepository
{
    public async Task<Lead?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Leads
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsOpenByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Leads
            .AsNoTracking()
            .AnyAsync(x => x.Email == email && !x.ConvertedToTenant, cancellationToken);
    }

    public async Task<List<Lead>> GetPaidNotConvertedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Leads
            .AsNoTracking()
            .Where(x => x.PaymentStatus == Domain.Enums.LeadPaymentStatus.Paid && !x.ConvertedToTenant)
            .OrderByDescending(x => x.PaidAtUtc)
            .ToListAsync(cancellationToken);
    }
}
