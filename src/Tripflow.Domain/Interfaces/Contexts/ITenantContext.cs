namespace Tripflow.Domain.Interfaces.Contexts;

public interface ITenantContext
{
    Guid? TenantId { get; }
    bool HasTenant { get; }
}