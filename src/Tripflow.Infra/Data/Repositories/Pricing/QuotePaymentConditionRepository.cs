using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Pricing;

public class QuotePaymentConditionRepository(TripflowDbContext context) : BaseRepository<QuotePaymentCondition>(context), IQuotePaymentConditionRepository
{
    public async Task<List<QuotePaymentCondition>> GetByPricingOptionIdAsync(
        Guid pricingOptionId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.QuotePaymentConditions
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.QuotePricingOptionId == pricingOptionId)
            .OrderBy(x => x.PaymentMethod)
            .ThenBy(x => x.Installments)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByPricingOptionIdAsync(
        Guid pricingOptionId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.QuotePaymentConditions
            .Where(x => x.TenantId == tenantId && x.QuotePricingOptionId == pricingOptionId)
            .ToListAsync(cancellationToken);

        if (existing.Count == 0)
            return;

        _context.QuotePaymentConditions.RemoveRange(existing);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<QuotePaymentCondition> conditions,
        CancellationToken cancellationToken = default)
    {
        await _context.QuotePaymentConditions.AddRangeAsync(conditions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
