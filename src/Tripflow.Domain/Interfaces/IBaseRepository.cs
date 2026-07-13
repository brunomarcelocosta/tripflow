using System.Linq.Expressions;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities;

namespace Tripflow.Domain.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAllAsync(
      Expression<Func<T, bool>>? filter = null,
      Expression<Func<T, object>>? orderBy = null,
      bool desc = false,
      params Expression<Func<T, object>>[] includes);

    Task<PagedResult<T>> GetPagedAsync(
        Expression<Func<T, bool>>? filter,
        int page = 1,
        int pageSize = 10,
        Expression<Func<T, object>>? orderBy = null,
        bool desc = false,
        params Expression<Func<T, object>>[] includes);

    Task<T?> GetByIdAsync(
        Guid id,
        params Expression<Func<T, object>>[] includes);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
