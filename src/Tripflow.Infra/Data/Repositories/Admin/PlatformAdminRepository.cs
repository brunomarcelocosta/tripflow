using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Admin;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Infra.Data.Repositories.Admin;

public sealed class PlatformAdminRepository(TripflowDbContext context)
    : IPlatformAdminRepository
{
    private static readonly Guid PlatformTenantId = TripflowDbSeedData.PlatformTenantId;

    public async Task<PagedResult<Tenant>> GetTenantsPagedAsync(
        Expression<Func<Tenant, bool>>? filter,
        Expression<Func<Tenant, object>>? orderBy,
        bool sortDesc,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Tenant> query = context.Tenants
            .AsNoTracking()
            .Where(x => x.Id != PlatformTenantId);

        if (filter is not null)
            query = query.Where(filter);

        query = query
            .Include(x => x.CurrentSubscription!)
                .ThenInclude(x => x.SubscriptionPlan);

        var totalItems = await query.CountAsync(cancellationToken);

        if (orderBy is not null)
        {
            query = sortDesc
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAtUtc);
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Tenant>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<Dictionary<Guid, AdminTenantMetrics>> GetTenantMetricsAsync(
        IEnumerable<Guid> tenantIds,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var ids = tenantIds.Distinct().ToList();

        if (ids.Count == 0)
            return [];

        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var userStats = await context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => ids.Contains(x.TenantId) && !x.IsDeleted)
            .GroupBy(x => x.TenantId)
            .Select(g => new
            {
                TenantId = g.Key,
                UsersCount = g.Count(),
                ActiveUsersCount = g.Count(x => x.Status == UserStatus.Active)
            })
            .ToListAsync(cancellationToken);

        var quoteStats = await context.Quotes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => ids.Contains(x.TenantId) && !x.IsDeleted &&
                        x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd)
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var proposalStats = await context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => ids.Contains(x.TenantId) && !x.IsDeleted &&
                        x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd)
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var paymentStats = await context.Payments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => ids.Contains(x.TenantId) && !x.IsDeleted &&
                        x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd)
            .GroupBy(x => x.TenantId)
            .Select(g => new
            {
                TenantId = g.Key,
                Count = g.Count(),
                GrossAmount = g.Sum(x => x.GrossAmount)
            })
            .ToListAsync(cancellationToken);

        var result = new Dictionary<Guid, AdminTenantMetrics>();

        foreach (var id in ids)
        {
            var users = userStats.FirstOrDefault(x => x.TenantId == id);
            var quotes = quoteStats.FirstOrDefault(x => x.TenantId == id);
            var proposals = proposalStats.FirstOrDefault(x => x.TenantId == id);
            var payments = paymentStats.FirstOrDefault(x => x.TenantId == id);

            result[id] = new AdminTenantMetrics(
                users?.UsersCount ?? 0,
                users?.ActiveUsersCount ?? 0,
                quotes?.Count ?? 0,
                proposals?.Count ?? 0,
                payments?.Count ?? 0,
                payments?.GrossAmount ?? 0);
        }

        return result;
    }

    public async Task<Tenant?> GetTenantWithDetailsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .AsNoTracking()
            .Include(x => x.Branding)
            .Include(x => x.CommercialSettings)
            .Include(x => x.CurrentSubscription!)
                .ThenInclude(x => x.SubscriptionPlan)
            .FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> GetRecentUsersByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Quote>> GetRecentQuotesByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.Quotes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetRecentPaymentsByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.Payments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminDashboardOverviewData> GetDashboardOverviewAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var tenantsQuery = context.Tenants
            .AsNoTracking()
            .Where(x => x.Id != PlatformTenantId);

        var totalTenants = await tenantsQuery.CountAsync(cancellationToken);
        var activeTenants = await tenantsQuery.CountAsync(x => x.Status == TenantStatus.Active, cancellationToken);
        var trialTenants = await tenantsQuery.CountAsync(x => x.Status == TenantStatus.Trial, cancellationToken);
        var suspendedTenants = await tenantsQuery.CountAsync(x => x.Status == TenantStatus.Suspended, cancellationToken);
        var cancelledTenants = await tenantsQuery.CountAsync(x => x.Status == TenantStatus.Cancelled, cancellationToken);

        var usersQuery = context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId != PlatformTenantId && !x.IsDeleted);

        var totalUsers = await usersQuery.CountAsync(cancellationToken);
        var activeUsers = await usersQuery.CountAsync(x => x.Status == UserStatus.Active, cancellationToken);

        var quotesCurrentMonth = await context.Quotes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                             x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd,
                cancellationToken);

        var proposalsCurrentMonth = await context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                             x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd,
                cancellationToken);

        var approvedProposalsCurrentMonth = await context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                             x.ApprovedAtUtc >= monthStart && x.ApprovedAtUtc < monthEnd,
                cancellationToken);

        var paymentsQuery = context.Payments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                        x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd);

        var paymentsCurrentMonth = await paymentsQuery.CountAsync(cancellationToken);
        var paymentsGrossAmountCurrentMonth = await paymentsQuery.SumAsync(x => x.GrossAmount, cancellationToken);
        var paymentsNetAmountCurrentMonth = await paymentsQuery.SumAsync(x => x.NetAmount ?? 0, cancellationToken);

        var leadsCurrentMonth = await context.Leads
            .AsNoTracking()
            .CountAsync(x => !x.IsDeleted &&
                             x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd,
                cancellationToken);

        var convertedLeadsCurrentMonth = await context.Leads
            .AsNoTracking()
            .CountAsync(x => !x.IsDeleted && x.ConvertedToTenant &&
                             x.UpdatedAtUtc >= monthStart && x.UpdatedAtUtc < monthEnd,
                cancellationToken);

        var planDistribution = await context.TenantSubscriptions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.TenantId != PlatformTenantId && !x.IsDeleted)
            .GroupBy(x => new { x.SubscriptionPlanId, x.SubscriptionPlan.Name })
            .Select(g => new AdminPlanDistributionRow(
                g.Key.SubscriptionPlanId,
                g.Key.Name,
                g.Count()))
            .ToListAsync(cancellationToken);

        var monthlyMetrics = new List<AdminMonthlyMetricRow>();

        for (var i = 5; i >= 0; i--)
        {
            var periodStart = monthStart.AddMonths(-i);
            var periodEnd = periodStart.AddMonths(1);

            var quotes = await context.Quotes
                .IgnoreQueryFilters()
                .AsNoTracking()
                .CountAsync(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                                 x.CreatedAtUtc >= periodStart && x.CreatedAtUtc < periodEnd,
                    cancellationToken);

            var proposals = await context.Proposals
                .IgnoreQueryFilters()
                .AsNoTracking()
                .CountAsync(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                                 x.CreatedAtUtc >= periodStart && x.CreatedAtUtc < periodEnd,
                    cancellationToken);

            var paymentsData = await context.Payments
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                            x.CreatedAtUtc >= periodStart && x.CreatedAtUtc < periodEnd)
                .Select(x => x.GrossAmount)
                .ToListAsync(cancellationToken);

            var newTenants = await context.Tenants
                .AsNoTracking()
                .CountAsync(x => x.Id != PlatformTenantId &&
                                 x.CreatedAtUtc >= periodStart && x.CreatedAtUtc < periodEnd,
                    cancellationToken);

            var newUsers = await context.UserProfiles
                .IgnoreQueryFilters()
                .AsNoTracking()
                .CountAsync(x => x.TenantId != PlatformTenantId && !x.IsDeleted &&
                                 x.CreatedAtUtc >= periodStart && x.CreatedAtUtc < periodEnd,
                    cancellationToken);

            monthlyMetrics.Add(new AdminMonthlyMetricRow(
                periodStart.Year,
                periodStart.Month,
                quotes,
                proposals,
                paymentsData.Count,
                paymentsData.Sum(),
                newTenants,
                newUsers));
        }

        return new AdminDashboardOverviewData(
            totalTenants,
            activeTenants,
            trialTenants,
            suspendedTenants,
            cancelledTenants,
            totalUsers,
            activeUsers,
            quotesCurrentMonth,
            proposalsCurrentMonth,
            approvedProposalsCurrentMonth,
            paymentsCurrentMonth,
            paymentsGrossAmountCurrentMonth,
            paymentsNetAmountCurrentMonth,
            leadsCurrentMonth,
            convertedLeadsCurrentMonth,
            planDistribution,
            monthlyMetrics);
    }

    public async Task<PagedResult<AdminTenantReportRow>> GetTenantReportPagedAsync(
        Expression<Func<Tenant, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Tenant> query = context.Tenants
            .AsNoTracking()
            .Where(x => x.Id != PlatformTenantId);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var tenants = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new { x.Id, x.TradeName, x.Status, x.CreatedAtUtc })
            .ToListAsync(cancellationToken);

        var tenantIds = tenants.Select(x => x.Id).ToList();

        var subscriptions = await context.TenantSubscriptions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.SubscriptionPlan)
            .Where(x => tenantIds.Contains(x.TenantId) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var userCounts = await context.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId) && !x.IsDeleted)
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var quoteCounts = await context.Quotes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId) && !x.IsDeleted)
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var proposalCounts = await context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId) && !x.IsDeleted)
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var paymentStats = await context.Payments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => tenantIds.Contains(x.TenantId) && !x.IsDeleted)
            .GroupBy(x => x.TenantId)
            .Select(g => new { TenantId = g.Key, Count = g.Count(), GrossAmount = g.Sum(x => x.GrossAmount) })
            .ToListAsync(cancellationToken);

        var items = tenants.Select(t =>
        {
            var sub = subscriptions.FirstOrDefault(x => x.TenantId == t.Id);
            return new AdminTenantReportRow(
                t.Id,
                t.TradeName,
                t.Status,
                sub?.SubscriptionPlan?.Name,
                userCounts.FirstOrDefault(x => x.TenantId == t.Id)?.Count ?? 0,
                quoteCounts.FirstOrDefault(x => x.TenantId == t.Id)?.Count ?? 0,
                proposalCounts.FirstOrDefault(x => x.TenantId == t.Id)?.Count ?? 0,
                paymentStats.FirstOrDefault(x => x.TenantId == t.Id)?.Count ?? 0,
                paymentStats.FirstOrDefault(x => x.TenantId == t.Id)?.GrossAmount ?? 0,
                t.CreatedAtUtc);
        }).ToList();

        return new PagedResult<AdminTenantReportRow>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<PagedResult<AdminUsageReportRow>> GetUsageReportPagedAsync(
        Expression<Func<TenantUsage, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TenantUsage> query = context.TenantUsages
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.PeriodYear)
            .ThenByDescending(x => x.PeriodMonth)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminUsageReportRow(
                x.TenantId,
                x.Tenant.TradeName,
                x.UsageType,
                x.PeriodYear,
                x.PeriodMonth,
                x.CurrentValue,
                x.LimitValue,
                x.LimitValue.HasValue && x.CurrentValue >= x.LimitValue.Value))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminUsageReportRow>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<PagedResult<AdminPaymentReportRow>> GetPaymentReportPagedAsync(
        Expression<Func<Payment, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Payment> query = context.Payments
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Where(x => !x.IsDeleted);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminPaymentReportRow(
                x.TenantId,
                x.Tenant.TradeName,
                x.Status,
                x.PaymentMethod,
                x.GrossAmount,
                x.NetAmount,
                x.PaidAtUtc,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminPaymentReportRow>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<PagedResult<AdminProposalReportRow>> GetProposalReportPagedAsync(
        Expression<Func<Proposal, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Proposal> query = context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Include(x => x.Quote)
                .ThenInclude(x => x!.Customer)
            .Where(x => !x.IsDeleted);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminProposalReportRow(
                x.TenantId,
                x.Tenant.TradeName,
                x.Status,
                x.ProposalNumber,
                x.Quote != null ? x.Quote.QuoteNumber : null,
                x.Quote != null && x.Quote.Customer != null ? x.Quote.Customer.FullName : null,
                x.CreatedAtUtc,
                x.ApprovedAtUtc,
                x.RejectedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminProposalReportRow>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<PagedResult<AdminSubscriptionReportRow>> GetSubscriptionReportPagedAsync(
        Expression<Func<TenantSubscription, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TenantSubscription> query = context.TenantSubscriptions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Include(x => x.SubscriptionPlan)
            .Where(x => !x.IsDeleted);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.StartedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminSubscriptionReportRow(
                x.TenantId,
                x.Tenant.TradeName,
                x.SubscriptionPlan.Name,
                x.Status,
                x.StartedAtUtc,
                x.ExpiresAtUtc,
                x.TrialEndsAtUtc,
                x.CancelledAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminSubscriptionReportRow>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<PagedResult<AuditLog>> GetAuditLogsPagedAsync(
        Expression<Func<AuditLog, bool>>? filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLog> query = context.AuditLogs
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Include(x => x.User);

        if (filter is not null)
            query = query.Where(filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLog>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<AuditLog?> GetAuditLogByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.AuditLogs
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Tenant)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetRecentAuditLogsByUserAsync(
        Guid userProfileId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.AuditLogs
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.UserId == userProfileId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Quote>> GetRecentQuotesByUserAsync(
        Guid userProfileId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.Quotes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedByUserId == userProfileId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Proposal>> GetRecentProposalsByTenantAsync(
        Guid tenantId,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await context.Proposals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
