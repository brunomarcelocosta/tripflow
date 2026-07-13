using System.Linq.Expressions;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Builders;

public static class AdminFilterExtensions
{
    public static Expression<Func<Tenant, bool>> ToExpression(this AdminTenantFilterRequest request)
    {
        Expression<Func<Tenant, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.LegalName.ToLower().Contains(search) ||
                x.TradeName.ToLower().Contains(search) ||
                (x.DocumentNumber != null && x.DocumentNumber.ToLower().Contains(search)) ||
                (x.Email != null && x.Email.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.LegalName))
        {
            var legalName = request.LegalName.Trim().ToLowerInvariant();
            filter = filter.And(x => x.LegalName.ToLower().Contains(legalName));
        }

        if (!string.IsNullOrWhiteSpace(request.TradeName))
        {
            var tradeName = request.TradeName.Trim().ToLowerInvariant();
            filter = filter.And(x => x.TradeName.ToLower().Contains(tradeName));
        }

        if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            var documentNumber = request.DocumentNumber.Trim();
            filter = filter.And(x => x.DocumentNumber == documentNumber);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            filter = filter.And(x => x.Email != null && x.Email.ToLower().Contains(email));
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        if (request.SubscriptionPlanId.HasValue)
        {
            var planId = request.SubscriptionPlanId.Value;
            filter = filter.And(x =>
                x.CurrentSubscription != null &&
                x.CurrentSubscription.SubscriptionPlanId == planId);
        }

        if (request.SubscriptionStatus.HasValue)
        {
            var subscriptionStatus = request.SubscriptionStatus.Value;
            filter = filter.And(x =>
                x.CurrentSubscription != null &&
                x.CurrentSubscription.Status == subscriptionStatus);
        }

        if (request.CreatedFromUtc.HasValue)
        {
            var from = request.CreatedFromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.CreatedToUtc.HasValue)
        {
            var to = request.CreatedToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        return filter;
    }

    public static Expression<Func<UserProfile, bool>> ToExpression(this AdminUserFilterRequest request)
    {
        Expression<Func<UserProfile, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.FullName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search) ||
                (x.Phone != null && x.Phone.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var fullName = request.FullName.Trim().ToLowerInvariant();
            filter = filter.And(x => x.FullName.ToLower().Contains(fullName));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            filter = filter.And(x => x.Email.ToLower().Contains(email));
        }

        if (request.Status.HasValue)
        {
            var status = request.Status.Value;
            filter = filter.And(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.RoleName))
        {
            var roleName = request.RoleName.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.UserRoles.Any(r => r.Role.Name.ToLower().Contains(roleName)));
        }

        if (request.CreatedFromUtc.HasValue)
        {
            var from = request.CreatedFromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.CreatedToUtc.HasValue)
        {
            var to = request.CreatedToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.TenantId == tenantId);
        }

        return filter;
    }

    public static Expression<Func<Tenant, bool>> ToTenantReportExpression(this AdminReportFilterRequest request)
    {
        Expression<Func<Tenant, bool>> filter = x => true;

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.Id == tenantId);
        }

        if (request.FromUtc.HasValue)
        {
            var from = request.FromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.ToUtc.HasValue)
        {
            var to = request.ToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<TenantStatus>(request.Status, true, out var status))
        {
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }

    public static Expression<Func<TenantUsage, bool>> ToUsageReportExpression(this AdminReportFilterRequest request)
    {
        Expression<Func<TenantUsage, bool>> filter = x => true;

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.TenantId == tenantId);
        }

        if (request.FromUtc.HasValue)
        {
            var from = request.FromUtc.Value;
            filter = filter.And(x =>
                x.PeriodYear > from.Year ||
                (x.PeriodYear == from.Year && x.PeriodMonth >= from.Month));
        }

        if (request.ToUtc.HasValue)
        {
            var to = request.ToUtc.Value;
            filter = filter.And(x =>
                x.PeriodYear < to.Year ||
                (x.PeriodYear == to.Year && x.PeriodMonth <= to.Month));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var usageType = request.Status.Trim();
            filter = filter.And(x => x.UsageType == usageType);
        }

        return filter;
    }

    public static Expression<Func<Payment, bool>> ToPaymentReportExpression(this AdminReportFilterRequest request)
    {
        Expression<Func<Payment, bool>> filter = x => true;

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.TenantId == tenantId);
        }

        if (request.FromUtc.HasValue)
        {
            var from = request.FromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.ToUtc.HasValue)
        {
            var to = request.ToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<PaymentStatus>(request.Status, true, out var status))
        {
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }

    public static Expression<Func<Proposal, bool>> ToProposalReportExpression(this AdminReportFilterRequest request)
    {
        Expression<Func<Proposal, bool>> filter = x => true;

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.TenantId == tenantId);
        }

        if (request.FromUtc.HasValue)
        {
            var from = request.FromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.ToUtc.HasValue)
        {
            var to = request.ToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ProposalStatus>(request.Status, true, out var status))
        {
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }

    public static Expression<Func<TenantSubscription, bool>> ToSubscriptionReportExpression(this AdminReportFilterRequest request)
    {
        Expression<Func<TenantSubscription, bool>> filter = x => true;

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.TenantId == tenantId);
        }

        if (request.FromUtc.HasValue)
        {
            var from = request.FromUtc.Value;
            filter = filter.And(x => x.StartedAtUtc >= from);
        }

        if (request.ToUtc.HasValue)
        {
            var to = request.ToUtc.Value;
            filter = filter.And(x => x.StartedAtUtc <= to);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<TenantSubscriptionStatus>(request.Status, true, out var status))
        {
            filter = filter.And(x => x.Status == status);
        }

        return filter;
    }

    public static Expression<Func<AuditLog, bool>> ToExpression(this AdminAuditLogFilterRequest request)
    {
        Expression<Func<AuditLog, bool>> filter = x => true;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLowerInvariant();
            filter = filter.And(x =>
                x.Action.ToLower().Contains(search) ||
                (x.EntityName != null && x.EntityName.ToLower().Contains(search)) ||
                (x.NewValuesJson != null && x.NewValuesJson.ToLower().Contains(search)));
        }

        if (request.TenantId.HasValue)
        {
            var tenantId = request.TenantId.Value;
            filter = filter.And(x => x.TenantId == tenantId);
        }

        if (request.UserProfileId.HasValue)
        {
            var userProfileId = request.UserProfileId.Value;
            filter = filter.And(x => x.UserId == userProfileId);
        }

        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            var action = request.Action.Trim();
            filter = filter.And(x => x.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(request.EntityName))
        {
            var entityName = request.EntityName.Trim();
            filter = filter.And(x => x.EntityName == entityName);
        }

        if (request.EntityId.HasValue)
        {
            var entityId = request.EntityId.Value;
            filter = filter.And(x => x.EntityId == entityId);
        }

        if (request.CreatedFromUtc.HasValue)
        {
            var from = request.CreatedFromUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc >= from);
        }

        if (request.CreatedToUtc.HasValue)
        {
            var to = request.CreatedToUtc.Value;
            filter = filter.And(x => x.CreatedAtUtc <= to);
        }

        return filter;
    }
}
