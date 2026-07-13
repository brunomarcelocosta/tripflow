namespace Tripflow.Application.DTOs.Requests.Subscriptions;

public sealed class UpdatePlanFeatureItem
{
    public string FeatureCode { get; init; } = string.Empty;
    public int? LimitValue { get; init; }
    public bool Enabled { get; init; } = true;
}

public sealed class UpdatePlanFeaturesRequest
{
    public IEnumerable<UpdatePlanFeatureItem> Features { get; init; } = [];
}
