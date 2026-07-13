using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Entities.Subscriptions;

public sealed class SubscriptionPlan : AuditableEntity
{
    private SubscriptionPlan() { }

    public SubscriptionPlan(string name, string? description, decimal? monthlyPrice, decimal? annualPrice, int? maxUsers, int? maxQuotesPerMonth, SubscriptionPlanStatus status, string createdBy)
    {
        Name = name;
        Description = description;
        MonthlyPrice = monthlyPrice;
        AnnualPrice = annualPrice;
        MaxUsers = maxUsers;
        MaxQuotesPerMonth = maxQuotesPerMonth;
        Status = status;
        SetCreated(createdBy);
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal? MonthlyPrice { get; private set; }
    public decimal? AnnualPrice { get; private set; }
    public int? MaxUsers { get; private set; }
    public int? MaxQuotesPerMonth { get; private set; }
    public SubscriptionPlanStatus Status { get; private set; }

    public List<PlanFeature> Features = [];

    public void Update(string name, string? description, decimal? monthlyPrice, decimal? annualPrice, int? maxUsers, int? maxQuotesPerMonth, SubscriptionPlanStatus status, string updatedBy)
    {
        Name = name;
        Description = description;
        MonthlyPrice = monthlyPrice;
        AnnualPrice = annualPrice;
        MaxUsers = maxUsers;
        MaxQuotesPerMonth = maxQuotesPerMonth;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = SubscriptionPlanStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Inactivate(string updatedBy)
    {
        Status = SubscriptionPlanStatus.Inactive;
        SetUpdated(updatedBy);
    }

    public void Deprecate(string updatedBy)
    {
        Status = SubscriptionPlanStatus.Deprecated;
        SetUpdated(updatedBy);
    }

    public bool CanBeAssigned() => Status != SubscriptionPlanStatus.Deprecated;
}
