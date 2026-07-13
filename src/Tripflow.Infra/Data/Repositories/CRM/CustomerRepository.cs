using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.CRM;

public class CustomerRepository(TripflowDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
{
    public async Task<Customer?> GetByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<Customer?> GetTrackedByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public async Task<bool> ExistsByDocumentNumberAsync(
        Guid tenantId,
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .AnyAsync(x => x.TenantId == tenantId && x.DocumentNumber == documentNumber, cancellationToken);
    }

    public async Task<bool> ExistsByDocumentNumberExceptIdAsync(
        Guid tenantId,
        string documentNumber,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .AnyAsync(
                x => x.TenantId == tenantId
                  && x.DocumentNumber == documentNumber
                  && x.Id != customerId,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .AnyAsync(x => x.TenantId == tenantId && x.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailExceptIdAsync(
        Guid tenantId,
        string email,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .AnyAsync(
                x => x.TenantId == tenantId
                  && x.Email == email
                  && x.Id != customerId,
                cancellationToken);
    }
}
