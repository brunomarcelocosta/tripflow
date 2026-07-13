using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Infra.Context;

public sealed class TenantContext : ITenantContextSetter
{
    public Guid? TenantId { get; private set; }

    public bool HasTenant => TenantId.HasValue;

    public void SetTenant(Guid tenantId)
    {
        TenantId = tenantId;
    }

    public void Clear()
    {
        TenantId = null;
    }
}
