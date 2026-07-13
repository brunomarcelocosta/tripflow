using System.Linq.Expressions;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Identity;

namespace Tripflow.Domain.Interfaces;

public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    Task<UserProfile?> GetByIdentityProviderUserIdAsync(
        string identityProviderUserId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetTrackedByIdentityProviderUserIdAsync(
        string identityProviderUserId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailInTenantAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByIdInTenantAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetTrackedByIdInTenantAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByIdForAdminAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> GetTrackedByIdForAdminAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<UserProfile>> GetPagedByTenantForAdminAsync(
        Guid tenantId,
        System.Linq.Expressions.Expression<Func<UserProfile, bool>>? filter,
        System.Linq.Expressions.Expression<Func<UserProfile, object>>? orderBy,
        bool sortDesc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<UserProfile>> GetPagedForAdminAsync(
        Guid? tenantId,
        System.Linq.Expressions.Expression<Func<UserProfile, bool>>? filter,
        System.Linq.Expressions.Expression<Func<UserProfile, object>>? orderBy,
        bool sortDesc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountActivePlatformAdminsAsync(
        CancellationToken cancellationToken = default);

    Task<bool> IsPlatformAdminAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
