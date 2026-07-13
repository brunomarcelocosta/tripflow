using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Infra.Data.Repositories.Identity;

public class UserProfileRepository(TripflowDbContext context) : BaseRepository<UserProfile>(context), IUserProfileRepository
{
    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private IQueryable<UserProfile> WithRolesAndPermissions(IQueryable<UserProfile> query) =>
        query
            .Include(x => x.Tenant)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission);

    public async Task<UserProfile?> GetByIdentityProviderUserIdAsync(
        string identityProviderUserId,
        CancellationToken cancellationToken = default)
    {
        return await WithRolesAndPermissions(
                _context.UserProfiles
                    .IgnoreQueryFilters()
                    .AsNoTracking())
            .FirstOrDefaultAsync(
                x => x.IdentityProviderUserId == identityProviderUserId,
                cancellationToken);
    }

    public async Task<UserProfile?> GetTrackedByIdentityProviderUserIdAsync(
        string identityProviderUserId,
        CancellationToken cancellationToken = default)
    {
        return await WithRolesAndPermissions(_context.UserProfiles.IgnoreQueryFilters())
            .FirstOrDefaultAsync(
                x => x.IdentityProviderUserId == identityProviderUserId,
                cancellationToken);
    }

    public async Task<UserProfile?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeEmail(email);

        return await _context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant)
            .FirstOrDefaultAsync(
                x => x.Email.ToLower() == normalized,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeEmail(email);

        return await _context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(
                x => x.Email.ToLower() == normalized,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmailInTenantAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeEmail(email);

        return await _context.UserProfiles
            .AsNoTracking()
            .AnyAsync(
                x => x.TenantId == tenantId && x.Email.ToLower() == normalized,
                cancellationToken);
    }

    public async Task<UserProfile?> GetByIdInTenantAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await WithRolesAndPermissions(_context.UserProfiles.AsNoTracking())
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == userId,
                cancellationToken);
    }

    public async Task<UserProfile?> GetTrackedByIdInTenantAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await WithRolesAndPermissions(_context.UserProfiles)
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId && x.Id == userId,
                cancellationToken);
    }

    public async Task<UserProfile?> GetByIdForAdminAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await WithRolesAndPermissions(
                _context.UserProfiles.IgnoreQueryFilters().AsNoTracking())
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<UserProfile?> GetTrackedByIdForAdminAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await WithRolesAndPermissions(_context.UserProfiles.IgnoreQueryFilters())
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<PagedResult<UserProfile>> GetPagedByTenantForAdminAsync(
        Guid tenantId,
        Expression<Func<UserProfile, bool>>? filter,
        Expression<Func<UserProfile, object>>? orderBy,
        bool sortDesc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UserProfile> query = WithRolesAndPermissions(
                _context.UserProfiles.IgnoreQueryFilters().AsNoTracking())
            .Where(x => x.TenantId == tenantId && !x.IsDeleted);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        if (orderBy is not null)
        {
            query = sortDesc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAtUtc);
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserProfile>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<PagedResult<UserProfile>> GetPagedForAdminAsync(
        Guid? tenantId,
        Expression<Func<UserProfile, bool>>? filter,
        Expression<Func<UserProfile, object>>? orderBy,
        bool sortDesc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UserProfile> query = WithRolesAndPermissions(
                _context.UserProfiles.IgnoreQueryFilters().AsNoTracking())
            .Where(x => !x.IsDeleted);

        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        if (orderBy is not null)
        {
            query = sortDesc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAtUtc);
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserProfile>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<int> CountActivePlatformAdminsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(
                x => x.TenantId == TripflowDbSeedData.PlatformTenantId &&
                     !x.IsDeleted &&
                     x.Status == UserStatus.Active &&
                     x.UserRoles.Any(r => r.Role.Name == TripflowDbSeedData.Roles.PlatformAdmin),
                cancellationToken);
    }

    public async Task<bool> IsPlatformAdminAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == userId &&
                     x.TenantId == TripflowDbSeedData.PlatformTenantId &&
                     !x.IsDeleted &&
                     x.UserRoles.Any(r => r.Role.Name == TripflowDbSeedData.Roles.PlatformAdmin),
                cancellationToken);
    }
}
