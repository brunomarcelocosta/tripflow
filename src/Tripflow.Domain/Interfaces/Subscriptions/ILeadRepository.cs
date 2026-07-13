using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Domain.Interfaces;

public interface ILeadRepository : IBaseRepository<Lead>
{
    Task<Lead?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsOpenByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<List<Lead>> GetPaidNotConvertedAsync(CancellationToken cancellationToken = default);
}
