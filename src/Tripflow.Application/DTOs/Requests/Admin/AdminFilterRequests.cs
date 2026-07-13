using Tripflow.Application.DTOs.Requests;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Admin;

public sealed class AdminTenantFilterRequest : FilterRequest
{
    public TenantStatus? Status { get; set; }
    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public Guid? SubscriptionPlanId { get; set; }
    public TenantSubscriptionStatus? SubscriptionStatus { get; set; }
    public DateTime? CreatedFromUtc { get; set; }
    public DateTime? CreatedToUtc { get; set; }
}

public sealed class AdminUserFilterRequest : FilterRequest
{
    public Guid? TenantId { get; set; }
    public UserStatus? Status { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? RoleName { get; set; }
    public DateTime? CreatedFromUtc { get; set; }
    public DateTime? CreatedToUtc { get; set; }
}

public sealed class AdminReportFilterRequest : FilterRequest
{
    public Guid? TenantId { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public string? Status { get; set; }
}

public sealed class AdminAuditLogFilterRequest : FilterRequest
{
    public Guid? TenantId { get; set; }
    public Guid? UserProfileId { get; set; }
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime? CreatedFromUtc { get; set; }
    public DateTime? CreatedToUtc { get; set; }
}
