namespace Tripflow.Domain.Interfaces;

public interface ITenantRoleProvisioningService
{
    Task ProvisionDefaultRolesAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
