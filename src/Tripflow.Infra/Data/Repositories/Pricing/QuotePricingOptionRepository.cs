using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Pricing;

public class QuotePricingOptionRepository(TripflowDbContext context) : BaseRepository<QuotePricingOption>(context), IQuotePricingOptionRepository
{
    public async Task<List<QuotePricingOption>> GetByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuotePricingOptions
            .AsNoTracking()
            .Include(x => x.PaymentConditions)
            .Where(x => x.TenantId == tenantId && x.QuoteId == quoteId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<QuotePricingOption?> GetByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuotePricingOptions
            .AsNoTracking()
            .Include(x => x.PaymentConditions)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<QuotePricingOption?> GetTrackedByIdAndQuoteAsync(
        Guid id,
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuotePricingOptions
            .Include(x => x.PaymentConditions)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.QuoteId == quoteId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<List<QuotePricingOption>> GetTrackedByQuoteIdAsync(
        Guid quoteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuotePricingOptions
            .Where(x => x.TenantId == tenantId && x.QuoteId == quoteId)
            .ToListAsync(cancellationToken);
    }
}
