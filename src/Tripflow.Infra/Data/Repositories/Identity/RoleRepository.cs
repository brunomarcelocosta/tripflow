using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Identity;

public class RoleRepository(TripflowDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    private IQueryable<Role> WithPermissions(IQueryable<Role> query) =>
        query
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission);

    public async Task<Role?> GetByNameAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await WithPermissions(_context.Roles.AsNoTracking())
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Name == name,
                cancellationToken);
    }

    public async Task<List<Role>> GetByIdsAsync(
        Guid tenantId,
        IReadOnlyList<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        if (roleIds.Count == 0)
            return [];

        return await WithPermissions(_context.Roles.AsNoTracking())
            .Where(x => x.TenantId == tenantId && roleIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Role>> GetByNamesAsync(
        Guid tenantId,
        IReadOnlyList<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        if (roleNames.Count == 0)
            return [];

        return await WithPermissions(_context.Roles.AsNoTracking())
            .Where(x => x.TenantId == tenantId && roleNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Role>> GetAllByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await WithPermissions(_context.Roles.AsNoTracking())
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
