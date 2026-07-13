namespace Tripflow.Domain.Interfaces.Contexts;

public interface ITenantContextSetter : ITenantContext
{
    void SetTenant(Guid tenantId);
    void Clear();
}