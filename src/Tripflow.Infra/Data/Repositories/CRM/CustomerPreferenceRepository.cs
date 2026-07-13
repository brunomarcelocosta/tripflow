using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.CRM;

public class CustomerPreferenceRepository(TripflowDbContext context) : BaseRepository<CustomerPreference>(context), ICustomerPreferenceRepository
{
    public async Task<CustomerPreference?> GetByCustomerAndTenantAsync(
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId && x.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<CustomerPreference?> GetTrackedByCustomerAndTenantAsync(
        Guid customerId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerPreferences
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId && x.TenantId == tenantId,
                cancellationToken);
    }
}
