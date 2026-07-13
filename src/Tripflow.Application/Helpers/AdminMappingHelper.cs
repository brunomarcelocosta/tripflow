using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Domain.Entities.Admin;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces.Admin;

namespace Tripflow.Application.Helpers;

public static class AdminMappingHelper
{
    public static AdminTenantResponse MapTenant(
        Tenant tenant,
        AdminTenantMetrics? metrics = null)
    {
        var subscription = tenant.CurrentSubscription;

        return new AdminTenantResponse
        {
            Id = tenant.Id,
            LegalName = tenant.LegalName,
            TradeName = tenant.TradeName,
            DocumentNumber = tenant.DocumentNumber,
            Email = tenant.Email,
            Phone = tenant.Phone,
            Status = tenant.Status,
            SubscriptionPlanName = subscription?.SubscriptionPlan?.Name,
            SubscriptionStatus = subscription?.Status,
            UsersCount = metrics?.UsersCount ?? 0,
            ActiveUsersCount = metrics?.ActiveUsersCount ?? 0,
            QuotesCurrentMonth = metrics?.QuotesCurrentMonth ?? 0,
            ProposalsCurrentMonth = metrics?.ProposalsCurrentMonth ?? 0,
            PaymentsCurrentMonth = metrics?.PaymentsCurrentMonth ?? 0,
            PaymentsGrossAmountCurrentMonth = metrics?.PaymentsGrossAmountCurrentMonth ?? 0,
            CreatedAtUtc = tenant.CreatedAtUtc,
            UpdatedAtUtc = tenant.UpdatedAtUtc
        };
    }

    public static TenantBrandingResponse? MapBranding(TenantBranding? branding)
    {
        if (branding is null)
            return null;

        return new TenantBrandingResponse
        {
            Id = branding.Id,
            TenantId = branding.TenantId,
            LogoUrl = branding.LogoUrl,
            PrimaryColor = branding.PrimaryColor,
            SecondaryColor = branding.SecondaryColor,
            TextColor = branding.TextColor,
            ProposalFooter = branding.ProposalFooter,
            CreatedAtUtc = branding.CreatedAtUtc,
            UpdatedAtUtc = branding.UpdatedAtUtc
        };
    }

    public static TenantCommercialSettingsResponse? MapCommercialSettings(TenantCommercialSettings? settings)
    {
        if (settings is null)
            return null;

        return new TenantCommercialSettingsResponse
        {
            Id = settings.Id,
            TenantId = settings.TenantId,
            CommercialEmail = settings.CommercialEmail,
            CommercialPhone = settings.CommercialPhone,
            WhatsApp = settings.WhatsApp,
            Instagram = settings.Instagram,
            Website = settings.Website,
            DefaultTerms = settings.DefaultTerms,
            DefaultImportantNotes = settings.DefaultImportantNotes,
            DefaultProposalExpirationHours = settings.DefaultProposalExpirationHours,
            CreatedAtUtc = settings.CreatedAtUtc,
            UpdatedAtUtc = settings.UpdatedAtUtc
        };
    }

    public static TenantSubscriptionResponse? MapSubscription(TenantSubscription? subscription)
    {
        if (subscription is null)
            return null;

        return new TenantSubscriptionResponse
        {
            Id = subscription.Id,
            TenantId = subscription.TenantId,
            SubscriptionPlanId = subscription.SubscriptionPlanId,
            SubscriptionPlanName = subscription.SubscriptionPlan?.Name ?? string.Empty,
            Status = subscription.Status,
            StartedAtUtc = subscription.StartedAtUtc,
            ExpiresAtUtc = subscription.ExpiresAtUtc,
            TrialEndsAtUtc = subscription.TrialEndsAtUtc,
            CancelledAtUtc = subscription.CancelledAtUtc,
            CreatedAtUtc = subscription.CreatedAtUtc,
            UpdatedAtUtc = subscription.UpdatedAtUtc
        };
    }

    public static TenantUsageResponse MapUsage(TenantUsage usage)
        => new()
        {
            Id = usage.Id,
            TenantId = usage.TenantId,
            UsageType = usage.UsageType,
            PeriodYear = usage.PeriodYear,
            PeriodMonth = usage.PeriodMonth,
            CurrentValue = usage.CurrentValue,
            LimitValue = usage.LimitValue,
            UpdatedAtUtc = usage.UpdatedAtUtc
        };

    public static AdminRecentUserSummaryResponse MapRecentUser(UserProfile user)
        => new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Status = user.Status,
            Roles = user.UserRoles.Select(x => x.Role.Name).ToList(),
            CreatedAtUtc = user.CreatedAtUtc
        };

    public static AdminRecentQuoteSummaryResponse MapRecentQuote(Quote quote)
        => new()
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            Title = quote.Title,
            Status = quote.Status,
            CreatedAtUtc = quote.CreatedAtUtc
        };

    public static AdminRecentPaymentSummaryResponse MapRecentPayment(Payment payment)
        => new()
        {
            Id = payment.Id,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            GrossAmount = payment.GrossAmount,
            NetAmount = payment.NetAmount,
            PaidAtUtc = payment.PaidAtUtc,
            CreatedAtUtc = payment.CreatedAtUtc
        };

    public static AdminUserResponse MapUser(UserProfile user)
        => new()
        {
            Id = user.Id,
            TenantId = user.TenantId,
            TenantTradeName = user.Tenant?.TradeName ?? string.Empty,
            IdentityProviderUserId = user.IdentityProviderUserId,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status,
            Roles = user.UserRoles.Select(x => x.Role.Name).ToList(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };

    public static IEnumerable<string> MapPermissions(UserProfile user)
        => user.UserRoles
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

    public static AdminAuditLogSummaryResponse MapAuditLogSummary(AuditLog auditLog)
        => new()
        {
            Id = auditLog.Id,
            Action = auditLog.Action,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            Description = auditLog.NewValuesJson,
            CreatedAtUtc = auditLog.CreatedAtUtc
        };

    public static AdminRecentProposalSummaryResponse MapRecentProposal(Proposal proposal)
        => new()
        {
            Id = proposal.Id,
            ProposalNumber = proposal.ProposalNumber,
            Status = proposal.Status,
            CreatedAtUtc = proposal.CreatedAtUtc,
            ApprovedAtUtc = proposal.ApprovedAtUtc
        };

    public static AdminDashboardOverviewResponse MapDashboard(AdminDashboardOverviewData data)
        => new()
        {
            TotalTenants = data.TotalTenants,
            ActiveTenants = data.ActiveTenants,
            TrialTenants = data.TrialTenants,
            SuspendedTenants = data.SuspendedTenants,
            CancelledTenants = data.CancelledTenants,
            TotalUsers = data.TotalUsers,
            ActiveUsers = data.ActiveUsers,
            QuotesCurrentMonth = data.QuotesCurrentMonth,
            ProposalsCurrentMonth = data.ProposalsCurrentMonth,
            ApprovedProposalsCurrentMonth = data.ApprovedProposalsCurrentMonth,
            PaymentsCurrentMonth = data.PaymentsCurrentMonth,
            PaymentsGrossAmountCurrentMonth = data.PaymentsGrossAmountCurrentMonth,
            PaymentsNetAmountCurrentMonth = data.PaymentsNetAmountCurrentMonth,
            LeadsCurrentMonth = data.LeadsCurrentMonth,
            ConvertedLeadsCurrentMonth = data.ConvertedLeadsCurrentMonth,
            PlanDistribution = data.PlanDistribution.Select(x => new AdminPlanDistributionResponse
            {
                SubscriptionPlanId = x.SubscriptionPlanId,
                PlanName = x.PlanName,
                TenantsCount = x.TenantsCount
            }).ToList(),
            MonthlyMetrics = data.MonthlyMetrics.Select(x => new AdminMonthlyMetricResponse
            {
                Year = x.Year,
                Month = x.Month,
                Quotes = x.Quotes,
                Proposals = x.Proposals,
                Payments = x.Payments,
                GrossAmount = x.GrossAmount,
                NewTenants = x.NewTenants,
                NewUsers = x.NewUsers
            }).ToList()
        };

    public static AdminTenantReportResponse MapTenantReport(AdminTenantReportRow row)
        => new()
        {
            TenantId = row.TenantId,
            TenantTradeName = row.TenantTradeName,
            Status = row.Status,
            PlanName = row.PlanName,
            UsersCount = row.UsersCount,
            QuotesCount = row.QuotesCount,
            ProposalsCount = row.ProposalsCount,
            PaymentsCount = row.PaymentsCount,
            GrossAmount = row.GrossAmount,
            CreatedAtUtc = row.CreatedAtUtc
        };

    public static AdminUsageReportResponse MapUsageReport(AdminUsageReportRow row)
        => new()
        {
            TenantId = row.TenantId,
            TenantTradeName = row.TenantTradeName,
            UsageType = row.UsageType,
            PeriodYear = row.PeriodYear,
            PeriodMonth = row.PeriodMonth,
            CurrentValue = row.CurrentValue,
            LimitValue = row.LimitValue,
            LimitReached = row.LimitReached
        };

    public static AdminPaymentReportResponse MapPaymentReport(AdminPaymentReportRow row)
        => new()
        {
            TenantId = row.TenantId,
            TenantTradeName = row.TenantTradeName,
            PaymentStatus = row.PaymentStatus,
            PaymentMethod = row.PaymentMethod,
            GrossAmount = row.GrossAmount,
            NetAmount = row.NetAmount,
            PaidAtUtc = row.PaidAtUtc,
            CreatedAtUtc = row.CreatedAtUtc
        };

    public static AdminProposalReportResponse MapProposalReport(AdminProposalReportRow row)
        => new()
        {
            TenantId = row.TenantId,
            TenantTradeName = row.TenantTradeName,
            ProposalStatus = row.ProposalStatus,
            ProposalNumber = row.ProposalNumber,
            QuoteNumber = row.QuoteNumber,
            CustomerName = row.CustomerName,
            CreatedAtUtc = row.CreatedAtUtc,
            ApprovedAtUtc = row.ApprovedAtUtc,
            RejectedAtUtc = row.RejectedAtUtc
        };

    public static AdminSubscriptionReportResponse MapSubscriptionReport(AdminSubscriptionReportRow row)
        => new()
        {
            TenantId = row.TenantId,
            TenantTradeName = row.TenantTradeName,
            PlanName = row.PlanName,
            SubscriptionStatus = row.SubscriptionStatus,
            StartedAtUtc = row.StartedAtUtc,
            ExpiresAtUtc = row.ExpiresAtUtc,
            TrialEndsAtUtc = row.TrialEndsAtUtc,
            CancelledAtUtc = row.CancelledAtUtc
        };

    public static SupportSessionResponse MapSupportSession(SupportSession session)
        => new()
        {
            Id = session.Id,
            AdminUserProfileId = session.AdminUserProfileId,
            AdminEmail = session.AdminUserProfile?.Email ?? string.Empty,
            TargetTenantId = session.TargetTenantId,
            TargetTenantTradeName = session.TargetTenant?.TradeName ?? string.Empty,
            Reason = session.Reason,
            StartedAtUtc = session.StartedAtUtc,
            EndedAtUtc = session.EndedAtUtc,
            IsActive = session.IsActive
        };

    public static AdminAuditLogResponse MapAuditLog(AuditLog auditLog)
        => new()
        {
            Id = auditLog.Id,
            TenantId = auditLog.TenantId,
            TenantTradeName = auditLog.Tenant?.TradeName,
            UserProfileId = auditLog.UserId,
            UserEmail = auditLog.User?.Email,
            Action = auditLog.Action,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            Description = auditLog.NewValuesJson,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            CreatedAtUtc = auditLog.CreatedAtUtc
        };
}
