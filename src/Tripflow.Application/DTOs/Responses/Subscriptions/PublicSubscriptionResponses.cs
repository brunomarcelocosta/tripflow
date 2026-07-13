namespace Tripflow.Application.DTOs.Responses.Subscriptions;

public sealed class PublicSubscriptionPlanResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal? MonthlyPrice { get; init; }
    public decimal? AnnualPrice { get; init; }
    public int? MaxUsers { get; init; }
    public int? MaxQuotesPerMonth { get; init; }
    public bool IsActive { get; init; }
    public bool RequiresContact { get; init; }
    public IReadOnlyList<PublicPlanFeatureResponse> Features { get; init; } = [];
}

public sealed class PublicPlanFeatureResponse
{
    public string Code { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public int? LimitValue { get; init; }
    public bool Enabled { get; init; }
}

public sealed class CreatePlatformCheckoutResponse
{
    public Guid LeadId { get; init; }
    public Guid CheckoutSessionId { get; init; }
    public string CheckoutUrl { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "BRL";
    public string Status { get; init; } = string.Empty;
}

public sealed class PlatformCheckoutStatusResponse
{
    public Guid CheckoutSessionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "BRL";
    public string PlanName { get; init; } = string.Empty;
    public DateTime? PaidAtUtc { get; init; }
}

public sealed class TenantEntitlementsResponse
{
    public Guid? SubscriptionPlanId { get; init; }
    public string? SubscriptionPlanName { get; init; }
    public string? SubscriptionStatus { get; init; }
    public int? MaxUsers { get; init; }
    public int? MaxQuotesPerMonth { get; init; }
    public IReadOnlyList<PublicPlanFeatureResponse> Features { get; init; } = [];
}
