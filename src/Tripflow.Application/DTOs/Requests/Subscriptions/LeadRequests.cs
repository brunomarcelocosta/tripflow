namespace Tripflow.Application.DTOs.Requests.Subscriptions;

public sealed class CreateLeadRequest
{
    public string CompanyName { get; init; } = string.Empty;
    public string ResponsibleName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? PlanOfInterest { get; init; }
    public string? Message { get; init; }
}

public sealed class LeadFilterRequest : FilterRequest
{
    public bool? ConvertedToTenant { get; set; }
    public string? Email { get; set; }
    public string? CompanyName { get; set; }
    public string? PlanOfInterest { get; set; }
    public DateTime? CreatedFromUtc { get; set; }
    public DateTime? CreatedToUtc { get; set; }
}

public sealed class ConvertLeadToTenantRequest
{
    public string LegalName { get; init; } = string.Empty;
    public string TradeName { get; init; } = string.Empty;
    public string? DocumentNumber { get; init; }
    public Guid? SubscriptionPlanId { get; init; }
}
