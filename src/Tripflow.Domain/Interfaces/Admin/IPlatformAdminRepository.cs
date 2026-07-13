using System.Linq.Expressions;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Interfaces.Admin;

public interface IPlatformAdminRepository
{
    Task<PagedResult<Tenant>> GetTenantsPagedAsync(
        Expression<Func<Tenant, bool>>? filter,
        Expression<Func<Tenant, object>>? orderBy,
        bool sortDesc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, AdminTenantMetrics>> GetTenantMetricsAsync(
        IEnumerable<Guid> tenantIds,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Tenant?> GetTenantWithDetailsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserProfile>> GetRecentUsersByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Quote>> GetRecentQuotesByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetRecentPaymentsByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default);

    Task<AdminDashboardOverviewData> GetDashboardOverviewAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminTenantReportRow>> GetTenantReportPagedAsync(
        Expression<Func<Tenant, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminUsageReportRow>> GetUsageReportPagedAsync(
        Expression<Func<TenantUsage, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminPaymentReportRow>> GetPaymentReportPagedAsync(
        Expression<Func<Payment, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminProposalReportRow>> GetProposalReportPagedAsync(
        Expression<Func<Proposal, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminSubscriptionReportRow>> GetSubscriptionReportPagedAsync(
        Expression<Func<TenantSubscription, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLog>> GetAuditLogsPagedAsync(
        Expression<Func<AuditLog, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AuditLog?> GetAuditLogByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLog>> GetRecentAuditLogsByUserAsync(
        Guid userProfileId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Quote>> GetRecentQuotesByUserAsync(
        Guid userProfileId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Proposal>> GetRecentProposalsByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default);
}

public sealed record AdminTenantMetrics(
    int UsersCount,
    int ActiveUsersCount,
    int QuotesCurrentMonth,
    int ProposalsCurrentMonth,
    int PaymentsCurrentMonth,
    decimal PaymentsGrossAmountCurrentMonth);

public sealed record AdminDashboardOverviewData(
    int TotalTenants,
    int ActiveTenants,
    int TrialTenants,
    int SuspendedTenants,
    int CancelledTenants,
    int TotalUsers,
    int ActiveUsers,
    int QuotesCurrentMonth,
    int ProposalsCurrentMonth,
    int ApprovedProposalsCurrentMonth,
    int PaymentsCurrentMonth,
    decimal PaymentsGrossAmountCurrentMonth,
    decimal PaymentsNetAmountCurrentMonth,
    int LeadsCurrentMonth,
    int ConvertedLeadsCurrentMonth,
    IReadOnlyList<AdminPlanDistributionRow> PlanDistribution,
    IReadOnlyList<AdminMonthlyMetricRow> MonthlyMetrics);

public sealed record AdminPlanDistributionRow(
    Guid? SubscriptionPlanId,
    string PlanName,
    int TenantsCount);

public sealed record AdminMonthlyMetricRow(
    int Year,
    int Month,
    int Quotes,
    int Proposals,
    int Payments,
    decimal GrossAmount,
    int NewTenants,
    int NewUsers);

public sealed record AdminTenantReportRow(
    Guid TenantId,
    string TenantTradeName,
    TenantStatus Status,
    string? PlanName,
    int UsersCount,
    int QuotesCount,
    int ProposalsCount,
    int PaymentsCount,
    decimal GrossAmount,
    DateTime CreatedAtUtc);

public sealed record AdminUsageReportRow(
    Guid TenantId,
    string TenantTradeName,
    string UsageType,
    int PeriodYear,
    int PeriodMonth,
    int CurrentValue,
    int? LimitValue,
    bool LimitReached);

public sealed record AdminPaymentReportRow(
    Guid TenantId,
    string TenantTradeName,
    PaymentStatus PaymentStatus,
    PaymentMethod PaymentMethod,
    decimal GrossAmount,
    decimal? NetAmount,
    DateTime? PaidAtUtc,
    DateTime CreatedAtUtc);

public sealed record AdminProposalReportRow(
    Guid TenantId,
    string TenantTradeName,
    ProposalStatus ProposalStatus,
    string ProposalNumber,
    string? QuoteNumber,
    string? CustomerName,
    DateTime CreatedAtUtc,
    DateTime? ApprovedAtUtc,
    DateTime? RejectedAtUtc);

public sealed record AdminSubscriptionReportRow(
    Guid TenantId,
    string TenantTradeName,
    string PlanName,
    TenantSubscriptionStatus SubscriptionStatus,
    DateTime StartedAtUtc,
    DateTime? ExpiresAtUtc,
    DateTime? TrialEndsAtUtc,
    DateTime? CancelledAtUtc);
