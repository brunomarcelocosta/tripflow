using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using TenantEntity = Tripflow.Domain.Entities.Tenants.Tenant;

namespace Tripflow.Infra.Data.Repositories.Tenants;

public class TenantRepository(TripflowDbContext context) : BaseRepository<TenantEntity>(context), ITenantRepository
{
    public async Task<TenantEntity?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        var normalized = documentNumber.Trim();

        return await context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.DocumentNumber == normalized,
                cancellationToken);
    }

    public async Task<TenantEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLower();

        return await context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Email != null && x.Email.Equals(normalized, StringComparison.CurrentCultureIgnoreCase),
                cancellationToken);
    }

    public async Task<bool> ExistsByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        var normalized = documentNumber.Trim();

        return await context.Tenants
            .AsNoTracking()
            .AnyAsync(
                x => x.DocumentNumber == normalized,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLower();

        return await context.Tenants
            .AsNoTracking()
            .AnyAsync(
                x => x.Email != null && x.Email.Equals(normalized, StringComparison.CurrentCultureIgnoreCase),
                cancellationToken);
    }

    public async Task<TenantEntity?> GetTrackedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}

