using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Domain.Interfaces;

public interface ICustomerLoyaltyAccountRepository : IBaseRepository<CustomerLoyaltyAccount>
{
    Task<List<CustomerLoyaltyAccount>> GetByCustomerIdAsync(Guid customerId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<CustomerLoyaltyAccount?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<CustomerLoyaltyAccount?> GetByCustomerAndTenantAsync(Guid id, Guid customerId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<CustomerLoyaltyAccount?> GetTrackedByCustomerAndTenantAsync(Guid id, Guid customerId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCustomerProgramAndAccountNumberAsync(Guid tenantId, Guid customerId, Guid loyaltyProgramId, string? accountNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCustomerProgramAndAccountNumberExceptIdAsync(Guid tenantId, Guid customerId, Guid loyaltyProgramId, string? accountNumber, Guid accountId, CancellationToken cancellationToken = default);
    Task<List<CustomerLoyaltyAccount>> GetSummaryAccountsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
