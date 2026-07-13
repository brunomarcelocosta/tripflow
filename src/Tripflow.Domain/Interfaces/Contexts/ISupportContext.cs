namespace Tripflow.Domain.Interfaces.Contexts;

public interface ISupportContext
{
    Guid? TargetTenantId { get; }
    bool IsSupportMode { get; }
}
