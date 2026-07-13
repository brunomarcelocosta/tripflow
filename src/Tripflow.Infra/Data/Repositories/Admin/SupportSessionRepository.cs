using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Admin;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;

namespace Tripflow.Infra.Data.Repositories.Admin;

public sealed class SupportSessionRepository(TripflowDbContext context)
    : BaseRepository<SupportSession>(context), ISupportSessionRepository
{
    public async Task<SupportSession?> GetCurrentByAdminUserAsync(
        Guid adminUserProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SupportSessions
            .AsNoTracking()
            .Include(x => x.AdminUserProfile)
            .Include(x => x.TargetTenant)
            .FirstOrDefaultAsync(
                x => x.AdminUserProfileId == adminUserProfileId && x.IsActive,
                cancellationToken);
    }

    public async Task<List<SupportSession>> GetByAdminUserAsync(
        Guid adminUserProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SupportSessions
            .AsNoTracking()
            .Include(x => x.AdminUserProfile)
            .Include(x => x.TargetTenant)
            .Where(x => x.AdminUserProfileId == adminUserProfileId)
            .OrderByDescending(x => x.StartedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupportSession?> GetTrackedCurrentByAdminUserAsync(
        Guid adminUserProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SupportSessions
            .FirstOrDefaultAsync(
                x => x.AdminUserProfileId == adminUserProfileId && x.IsActive,
                cancellationToken);
    }

    public async Task<PagedResult<SupportSession>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SupportSessions
            .AsNoTracking()
            .Include(x => x.AdminUserProfile)
            .Include(x => x.TargetTenant);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.StartedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<SupportSession>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }
}
