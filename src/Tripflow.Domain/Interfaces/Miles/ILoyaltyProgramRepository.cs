using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Domain.Interfaces;

public interface ILoyaltyProgramRepository : IBaseRepository<LoyaltyProgram>
{
    Task<LoyaltyProgram?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<LoyaltyProgram?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameExceptIdAsync(string name, Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasLinkedAccountsAsync(Guid id, CancellationToken cancellationToken = default);
}
