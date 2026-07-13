using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Admin;

public class AdminTenantResponse
{
    public Guid Id { get; init; }
    public string LegalName { get; init; } = string.Empty;
    public string TradeName { get; init; } = string.Empty;
    public string? DocumentNumber { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public TenantStatus Status { get; init; }
    public string? SubscriptionPlanName { get; init; }
    public TenantSubscriptionStatus? SubscriptionStatus { get; init; }
    public int UsersCount { get; init; }
    public int ActiveUsersCount { get; init; }
    public int QuotesCurrentMonth { get; init; }
    public int ProposalsCurrentMonth { get; init; }
    public int PaymentsCurrentMonth { get; init; }
    public decimal PaymentsGrossAmountCurrentMonth { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class AdminTenantDetailResponse : AdminTenantResponse
{
    public TenantBrandingResponse? Branding { get; init; }
    public TenantCommercialSettingsResponse? CommercialSettings { get; init; }
    public TenantSubscriptionResponse? Subscription { get; init; }
    public IEnumerable<TenantUsageResponse> CurrentMonthUsage { get; init; } = [];
    public IEnumerable<AdminRecentUserSummaryResponse> RecentUsers { get; init; } = [];
    public IEnumerable<AdminRecentQuoteSummaryResponse> RecentQuotes { get; init; } = [];
    public IEnumerable<AdminRecentPaymentSummaryResponse> RecentPayments { get; init; } = [];
}

public sealed class AdminRecentUserSummaryResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserStatus Status { get; init; }
    public IEnumerable<string> Roles { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class AdminRecentQuoteSummaryResponse
{
    public Guid Id { get; init; }
    public string QuoteNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public QuoteStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class AdminRecentPaymentSummaryResponse
{
    public Guid Id { get; init; }
    public PaymentStatus Status { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal? NetAmount { get; init; }
    public DateTime? PaidAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public class AdminUserResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = string.Empty;
    public string IdentityProviderUserId { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public UserStatus Status { get; init; }
    public IEnumerable<string> Roles { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class AdminUserDetailResponse : AdminUserResponse
{
    public IEnumerable<string> Permissions { get; init; } = [];
    public IEnumerable<AdminAuditLogSummaryResponse> RecentAuditLogs { get; init; } = [];
    public IEnumerable<AdminRecentQuoteSummaryResponse> RecentQuotesCreated { get; init; } = [];
    public IEnumerable<AdminRecentProposalSummaryResponse> RecentProposalsGenerated { get; init; } = [];
}

public sealed class AdminAuditLogSummaryResponse
{
    public Guid Id { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? EntityName { get; init; }
    public Guid? EntityId { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class AdminRecentProposalSummaryResponse
{
    public Guid Id { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public ProposalStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
}

public sealed class AdminDashboardOverviewResponse
{
    public int TotalTenants { get; init; }
    public int ActiveTenants { get; init; }
    public int TrialTenants { get; init; }
    public int SuspendedTenants { get; init; }
    public int CancelledTenants { get; init; }
    public int TotalUsers { get; init; }
    public int ActiveUsers { get; init; }
    public int QuotesCurrentMonth { get; init; }
    public int ProposalsCurrentMonth { get; init; }
    public int ApprovedProposalsCurrentMonth { get; init; }
    public int PaymentsCurrentMonth { get; init; }
    public decimal PaymentsGrossAmountCurrentMonth { get; init; }
    public decimal PaymentsNetAmountCurrentMonth { get; init; }
    public int LeadsCurrentMonth { get; init; }
    public int ConvertedLeadsCurrentMonth { get; init; }
    public IEnumerable<AdminPlanDistributionResponse> PlanDistribution { get; init; } = [];
    public IEnumerable<AdminMonthlyMetricResponse> MonthlyMetrics { get; init; } = [];
}

public sealed class AdminPlanDistributionResponse
{
    public Guid? SubscriptionPlanId { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public int TenantsCount { get; init; }
}

public sealed class AdminMonthlyMetricResponse
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int Quotes { get; init; }
    public int Proposals { get; init; }
    public int Payments { get; init; }
    public decimal GrossAmount { get; init; }
    public int NewTenants { get; init; }
    public int NewUsers { get; init; }
}

public sealed class AdminTenantReportResponse
{
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = string.Empty;
    public TenantStatus Status { get; init; }
    public string? PlanName { get; init; }
    public int UsersCount { get; init; }
    public int QuotesCount { get; init; }
    public int ProposalsCount { get; init; }
    public int PaymentsCount { get; init; }
    public decimal GrossAmount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class AdminUsageReportResponse
{
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = string.Empty;
    public string UsageType { get; init; } = string.Empty;
    public int PeriodYear { get; init; }
    public int PeriodMonth { get; init; }
    public int CurrentValue { get; init; }
    public int? LimitValue { get; init; }
    public bool LimitReached { get; init; }
}

public sealed class AdminPaymentReportResponse
{
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = string.Empty;
    public PaymentStatus PaymentStatus { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal? NetAmount { get; init; }
    public DateTime? PaidAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class AdminProposalReportResponse
{
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = string.Empty;
    public ProposalStatus ProposalStatus { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public string? QuoteNumber { get; init; }
    public string? CustomerName { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
    public DateTime? RejectedAtUtc { get; init; }
}

public sealed class AdminSubscriptionReportResponse
{
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = string.Empty;
    public string PlanName { get; init; } = string.Empty;
    public TenantSubscriptionStatus SubscriptionStatus { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? ExpiresAtUtc { get; init; }
    public DateTime? TrialEndsAtUtc { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
}

public sealed class SupportSessionResponse
{
    public Guid Id { get; init; }
    public Guid AdminUserProfileId { get; init; }
    public string AdminEmail { get; init; } = string.Empty;
    public Guid TargetTenantId { get; init; }
    public string TargetTenantTradeName { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public DateTime StartedAtUtc { get; init; }
    public DateTime? EndedAtUtc { get; init; }
    public bool IsActive { get; init; }
}

public sealed class AdminAuditLogResponse
{
    public Guid Id { get; init; }
    public Guid? TenantId { get; init; }
    public string? TenantTradeName { get; init; }
    public Guid? UserProfileId { get; init; }
    public string? UserEmail { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? EntityName { get; init; }
    public Guid? EntityId { get; init; }
    public string? Description { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
