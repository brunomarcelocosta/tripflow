using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Subscriptions;

public sealed class TenantUsage : BaseEntity, ITenantEntity
{
    private TenantUsage() { }

    public TenantUsage(Guid tenantId, string usageType, int periodYear, int periodMonth, int currentValue, int? limitValue)
    {
        TenantId = tenantId;
        UsageType = usageType;
        PeriodYear = periodYear;
        PeriodMonth = periodMonth;
        CurrentValue = currentValue;
        LimitValue = limitValue;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public string UsageType { get; private set; } = string.Empty;
    public int PeriodYear { get; private set; }
    public int PeriodMonth { get; private set; }
    public int CurrentValue { get; private set; }
    public int? LimitValue { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Increment(int amount = 1)
    {
        CurrentValue += amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Decrement(int amount = 1)
    {
        CurrentValue = Math.Max(0, CurrentValue - amount);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetLimit(int? limitValue)
    {
        LimitValue = limitValue;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public bool LimitReached() => LimitValue.HasValue && CurrentValue >= LimitValue.Value;

    public int? Remaining() => LimitValue.HasValue ? Math.Max(0, LimitValue.Value - CurrentValue) : null;
}
