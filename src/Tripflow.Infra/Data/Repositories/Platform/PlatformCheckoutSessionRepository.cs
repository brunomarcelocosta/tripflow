using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Platform;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Platform;

public class PlatformCheckoutSessionRepository(TripflowDbContext context)
    : BaseRepository<PlatformCheckoutSession>(context), IPlatformCheckoutSessionRepository
{
    public async Task<PlatformCheckoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PlatformCheckoutSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PlatformCheckoutSession?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PlatformCheckoutSessions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PlatformCheckoutSession?> GetByExternalCheckoutIdAsync(string externalCheckoutId, CancellationToken cancellationToken = default)
    {
        return await _context.PlatformCheckoutSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ExternalCheckoutId == externalCheckoutId, cancellationToken);
    }

    public async Task<PlatformCheckoutSession?> GetTrackedByExternalCheckoutIdAsync(string externalCheckoutId, CancellationToken cancellationToken = default)
    {
        return await _context.PlatformCheckoutSessions
            .FirstOrDefaultAsync(x => x.ExternalCheckoutId == externalCheckoutId, cancellationToken);
    }

    public async Task<List<PlatformCheckoutSession>> GetByLeadIdAsync(Guid leadId, CancellationToken cancellationToken = default)
    {
        return await _context.PlatformCheckoutSessions
            .AsNoTracking()
            .Where(x => x.LeadId == leadId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
