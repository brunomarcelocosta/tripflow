using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Data.Repositories.Identity;

public class PermissionRepository(TripflowDbContext context) : BaseRepository<Permission>(context), IPermissionRepository
{
    public async Task<List<Permission>> GetByCodesAsync(
        IReadOnlyList<string> codes,
        CancellationToken cancellationToken = default)
    {
        if (codes.Count == 0)
            return [];

        return await _context.Permissions
            .AsNoTracking()
            .Where(x => codes.Contains(x.Code))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }
}
