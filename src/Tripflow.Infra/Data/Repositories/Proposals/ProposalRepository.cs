using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Proposals;

public class ProposalRepository(TripflowDbContext context) : BaseRepository<Proposal>(context), IProposalRepository
{
    public async Task<Proposal?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Customer)
            .Include(p => p.QuotePricingOption)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Proposal?> GetTrackedByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Proposal?> GetByPublicTokenAsync(string publicToken, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Customer)
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Itinerary!)
                    .ThenInclude(i => i.Stops)
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Items)
            .Include(p => p.Quote)
                .ThenInclude(q => q!.FlightItems)
                    .ThenInclude(f => f.Segments)
            .Include(p => p.QuotePricingOption!)
                .ThenInclude(o => o.PaymentConditions)
            .Include(p => p.Tenant)
                .ThenInclude(t => t.Branding)
            .Include(p => p.Tenant)
                .ThenInclude(t => t.CommercialSettings)
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.PublicToken == publicToken, cancellationToken);
    }

    public async Task<Proposal?> GetTrackedByPublicTokenAsync(string publicToken, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .IgnoreQueryFilters()
            .Include(p => p.Quote)
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.PublicToken == publicToken, cancellationToken);
    }

    public async Task<Proposal?> GetFullByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Customer)
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Itinerary!)
                    .ThenInclude(i => i.Stops)
            .Include(p => p.Quote)
                .ThenInclude(q => q!.Items)
            .Include(p => p.Quote)
                .ThenInclude(q => q!.FlightItems)
                    .ThenInclude(f => f.Segments)
            .Include(p => p.QuotePricingOption!)
                .ThenInclude(o => o.PaymentConditions)
            .Include(p => p.Tenant)
                .ThenInclude(t => t.Branding)
            .Include(p => p.Tenant)
                .ThenInclude(t => t.CommercialSettings)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsByProposalNumberAsync(Guid tenantId, string proposalNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Proposals
            .AsNoTracking()
            .AnyAsync(x => x.TenantId == tenantId && x.ProposalNumber == proposalNumber, cancellationToken);
    }

    public async Task<string> GenerateNextProposalNumberAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var prefix = $"PR-{DateTime.UtcNow:yyyyMM}-";

        var existingNumbers = await _context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.ProposalNumber.StartsWith(prefix))
            .Select(x => x.ProposalNumber)
            .ToListAsync(cancellationToken);

        var maxSeq = 0;
        foreach (var number in existingNumbers)
        {
            var suffix = number[prefix.Length..];
            if (int.TryParse(suffix, out var parsed) && parsed > maxSeq)
                maxSeq = parsed;
        }

        return $"{prefix}{maxSeq + 1:D6}";
    }

    public async Task<Dictionary<Guid, (int VersionsCount, int EventsCount)>> GetAggregatesAsync(
        Guid tenantId,
        IEnumerable<Guid> proposalIds,
        CancellationToken cancellationToken = default)
    {
        var ids = proposalIds.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        var versions = await _context.ProposalVersions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && ids.Contains(x.ProposalId))
            .GroupBy(x => x.ProposalId)
            .Select(g => new { ProposalId = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        var events = await _context.ProposalEvents
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && ids.Contains(x.ProposalId))
            .GroupBy(x => x.ProposalId)
            .Select(g => new { ProposalId = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        return ids.ToDictionary(
            id => id,
            id => (
                versions.FirstOrDefault(v => v.ProposalId == id)?.Total ?? 0,
                events.FirstOrDefault(e => e.ProposalId == id)?.Total ?? 0));
    }
}
