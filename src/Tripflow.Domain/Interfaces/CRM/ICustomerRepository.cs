using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Domain.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<Customer?> GetTrackedByIdAndTenantAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentNumberAsync(
        Guid tenantId,
        string documentNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentNumberExceptIdAsync(
        Guid tenantId,
        string documentNumber,
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailExceptIdAsync(
        Guid tenantId,
        string email,
        Guid customerId,
        CancellationToken cancellationToken = default);
}
