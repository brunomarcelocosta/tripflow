using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Subscriptions;

public sealed class SubscriptionPlanFilterRequest : FilterRequest
{
    public SubscriptionPlanStatus? Status { get; set; }
}

public sealed class CreateSubscriptionPlanRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal? MonthlyPrice { get; init; }
    public decimal? AnnualPrice { get; init; }
    public int? MaxUsers { get; init; }
    public int? MaxQuotesPerMonth { get; init; }
    public SubscriptionPlanStatus Status { get; init; } = SubscriptionPlanStatus.Active;
}

public sealed class UpdateSubscriptionPlanRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal? MonthlyPrice { get; init; }
    public decimal? AnnualPrice { get; init; }
    public int? MaxUsers { get; init; }
    public int? MaxQuotesPerMonth { get; init; }
    public SubscriptionPlanStatus Status { get; init; } = SubscriptionPlanStatus.Active;
}
