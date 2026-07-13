namespace Tripflow.Application.DTOs.Requests.Subscriptions;

public sealed record CreatePlatformCheckoutRequest(
    Guid PlanId,
    string CompanyName,
    string ResponsibleName,
    string Email,
    string? Phone,
    string BillingCycle = "monthly");
