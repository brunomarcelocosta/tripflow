using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Subscriptions;

public sealed class PlanFeatureResponse
{
    public Guid Id { get; init; }
    public Guid SubscriptionPlanId { get; init; }
    public string FeatureCode { get; init; } = string.Empty;
    public int? LimitValue { get; init; }
    public bool Enabled { get; init; }
}

public sealed class SubscriptionPlanResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal? MonthlyPrice { get; init; }
    public decimal? AnnualPrice { get; init; }
    public int? MaxUsers { get; init; }
    public int? MaxQuotesPerMonth { get; init; }
    public SubscriptionPlanStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public IEnumerable<PlanFeatureResponse> Features { get; init; } = [];
}

public sealed class TenantSubscriptionResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid SubscriptionPlanId { get; init; }
    public string SubscriptionPlanName { get; init; } = string.Empty;
    public TenantSubscriptionStatus Status { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? ExpiresAtUtc { get; init; }
    public DateTime? TrialEndsAtUtc { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class TenantUsageResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string UsageType { get; init; } = string.Empty;
    public int PeriodYear { get; init; }
    public int PeriodMonth { get; init; }
    public int CurrentValue { get; init; }
    public int? LimitValue { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public bool LimitReached => LimitValue.HasValue && CurrentValue >= LimitValue.Value;
    public int? Remaining => LimitValue.HasValue ? Math.Max(0, LimitValue.Value - CurrentValue) : null;
}

public sealed class LeadResponse
{
    public Guid Id { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string ResponsibleName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? PlanOfInterest { get; init; }
    public string? Message { get; init; }
    public bool ConvertedToTenant { get; init; }
    public Guid? ConvertedTenantId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
