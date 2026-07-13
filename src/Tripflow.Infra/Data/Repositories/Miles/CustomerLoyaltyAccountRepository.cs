using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Miles;

public class CustomerLoyaltyAccountRepository(TripflowDbContext context) : BaseRepository<CustomerLoyaltyAccount>(context), ICustomerLoyaltyAccountRepository
{
    public async Task<List<CustomerLoyaltyAccount>> GetByCustomerIdAsync(Guid customerId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.Customer)
            .Where(x => x.TenantId == tenantId && x.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerLoyaltyAccount?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<CustomerLoyaltyAccount?> GetByCustomerAndTenantAsync(Guid id, Guid customerId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<CustomerLoyaltyAccount?> GetTrackedByCustomerAndTenantAsync(Guid id, Guid customerId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsByCustomerProgramAndAccountNumberAsync(Guid tenantId, Guid customerId, Guid loyaltyProgramId, string? accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .AnyAsync(x =>
                x.TenantId == tenantId
                && x.CustomerId == customerId
                && x.LoyaltyProgramId == loyaltyProgramId
                && x.AccountNumber == accountNumber,
                cancellationToken);
    }

    public async Task<bool> ExistsByCustomerProgramAndAccountNumberExceptIdAsync(Guid tenantId, Guid customerId, Guid loyaltyProgramId, string? accountNumber, Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .AnyAsync(x =>
                x.TenantId == tenantId
                && x.CustomerId == customerId
                && x.LoyaltyProgramId == loyaltyProgramId
                && x.AccountNumber == accountNumber
                && x.Id != accountId,
                cancellationToken);
    }

    public async Task<List<CustomerLoyaltyAccount>> GetSummaryAccountsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerLoyaltyAccounts
            .AsNoTracking()
            .Include(x => x.LoyaltyProgram)
            .Include(x => x.Customer)
            .Where(x => x.TenantId == tenantId && (x.CurrentBalance > 0 || x.Status == LoyaltyAccountStatus.Active))
            .ToListAsync(cancellationToken);
    }
}
