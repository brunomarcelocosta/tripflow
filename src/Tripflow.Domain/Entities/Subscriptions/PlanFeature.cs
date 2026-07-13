namespace Tripflow.Domain.Entities.Subscriptions;

public sealed class PlanFeature : BaseEntity
{
    private PlanFeature() { }

    public PlanFeature(Guid subscriptionPlanId, string featureCode, int? limitValue, bool enabled)
    {
        SubscriptionPlanId = subscriptionPlanId;
        FeatureCode = featureCode;
        LimitValue = limitValue;
        Enabled = enabled;
    }

    public Guid SubscriptionPlanId { get; private set; }
    public SubscriptionPlan SubscriptionPlan { get; private set; } = default!;

    public string FeatureCode { get; private set; } = default!;
    public int? LimitValue { get; private set; }
    public bool Enabled { get; private set; }

    public void Update(int? limitValue, bool enabled)
    {
        LimitValue = limitValue;
        Enabled = enabled;
    }
}
