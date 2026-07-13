using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Audit;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;
using FluentValidation;

namespace Tripflow.Application.UseCases.Admin;

public sealed class GetAdminTenantsUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IUserContext userContext) : IGetAdminTenantsUseCase
{
    public async Task<Result<PagedResponse<AdminTenantResponse>>> ExecuteAsync(
        AdminTenantFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminTenantResponse>>.Forbidden();

        var now = DateTime.UtcNow;
        var filter = request.ToExpression();
        var orderBy = AdminOrderByHelper.BuildTenant(request.SortBy);

        var paged = await platformAdminRepository.GetTenantsPagedAsync(
            filter,
            orderBy,
            request.SortDesc,
            request.Page,
            request.PageSize,
            cancellationToken);

        var tenantIds = paged.Items.Select(x => x.Id).ToList();
        var metrics = await platformAdminRepository.GetTenantMetricsAsync(
            tenantIds,
            now.Year,
            now.Month,
            cancellationToken);

        var mapped = paged.Items
            .Select(t => AdminMappingHelper.MapTenant(
                t,
                metrics.TryGetValue(t.Id, out var m) ? m : null))
            .ToList();

        return Result<PagedResponse<AdminTenantResponse>>.Ok(new PagedResponse<AdminTenantResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminTenantByIdUseCase(
    IPlatformAdminRepository platformAdminRepository,
    ITenantUsageRepository tenantUsageRepository,
    IUserContext userContext) : IGetAdminTenantByIdUseCase
{
    public async Task<Result<AdminTenantDetailResponse?>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminTenantDetailResponse?>.Forbidden();

        if (tenantId == TripflowDbSeedData.PlatformTenantId)
            return Result<AdminTenantDetailResponse?>.Failure("Tenant da plataforma não pode ser consultado por este endpoint.");

        var tenant = await platformAdminRepository.GetTenantWithDetailsAsync(tenantId, cancellationToken);

        if (tenant is null)
            return Result<AdminTenantDetailResponse?>.Ok(null);

        var now = DateTime.UtcNow;
        var metrics = await platformAdminRepository.GetTenantMetricsAsync(
            [tenantId],
            now.Year,
            now.Month,
            cancellationToken);

        metrics.TryGetValue(tenantId, out var tenantMetrics);

        var usage = await tenantUsageRepository.GetCurrentByTenantAsync(
            tenantId,
            now.Year,
            now.Month,
            cancellationToken);

        var recentUsers = await platformAdminRepository.GetRecentUsersByTenantAsync(tenantId, 5, cancellationToken);
        var recentQuotes = await platformAdminRepository.GetRecentQuotesByTenantAsync(tenantId, 5, cancellationToken);
        var recentPayments = await platformAdminRepository.GetRecentPaymentsByTenantAsync(tenantId, 5, cancellationToken);

        var baseResponse = AdminMappingHelper.MapTenant(tenant, tenantMetrics);

        var response = new AdminTenantDetailResponse
        {
            Id = baseResponse.Id,
            LegalName = baseResponse.LegalName,
            TradeName = baseResponse.TradeName,
            DocumentNumber = baseResponse.DocumentNumber,
            Email = baseResponse.Email,
            Phone = baseResponse.Phone,
            Status = baseResponse.Status,
            SubscriptionPlanName = baseResponse.SubscriptionPlanName,
            SubscriptionStatus = baseResponse.SubscriptionStatus,
            UsersCount = baseResponse.UsersCount,
            ActiveUsersCount = baseResponse.ActiveUsersCount,
            QuotesCurrentMonth = baseResponse.QuotesCurrentMonth,
            ProposalsCurrentMonth = baseResponse.ProposalsCurrentMonth,
            PaymentsCurrentMonth = baseResponse.PaymentsCurrentMonth,
            PaymentsGrossAmountCurrentMonth = baseResponse.PaymentsGrossAmountCurrentMonth,
            CreatedAtUtc = baseResponse.CreatedAtUtc,
            UpdatedAtUtc = baseResponse.UpdatedAtUtc,
            Branding = AdminMappingHelper.MapBranding(tenant.Branding),
            CommercialSettings = AdminMappingHelper.MapCommercialSettings(tenant.CommercialSettings),
            Subscription = AdminMappingHelper.MapSubscription(tenant.CurrentSubscription),
            CurrentMonthUsage = usage.Select(AdminMappingHelper.MapUsage).ToList(),
            RecentUsers = recentUsers.Select(AdminMappingHelper.MapRecentUser).ToList(),
            RecentQuotes = recentQuotes.Select(AdminMappingHelper.MapRecentQuote).ToList(),
            RecentPayments = recentPayments.Select(AdminMappingHelper.MapRecentPayment).ToList()
        };

        return Result<AdminTenantDetailResponse?>.Ok(response);
    }
}

public sealed class ActivateAdminTenantUseCase(
    ITenantRepository tenantRepository,
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : IActivateAdminTenantUseCase
{
    public async Task<Result<AdminTenantResponse?>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminTenantResponse?>.Forbidden();

        if (tenantId == TripflowDbSeedData.PlatformTenantId)
            return Result<AdminTenantResponse?>.Failure("Não é permitido alterar o tenant da plataforma.");

        var tenant = await tenantRepository.GetTrackedByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
            return Result<AdminTenantResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        tenant.Activate(updatedBy);
        await tenantRepository.UpdateAsync(tenant, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            tenantId,
            null,
            AuditActions.TenantActivated,
            nameof(Tenant),
            tenantId,
            $"Tenant {tenant.TradeName} ativado."), cancellationToken);

        var refreshed = await platformAdminRepository.GetTenantWithDetailsAsync(tenantId, cancellationToken);
        var now = DateTime.UtcNow;
        var metrics = await platformAdminRepository.GetTenantMetricsAsync([tenantId], now.Year, now.Month, cancellationToken);
        metrics.TryGetValue(tenantId, out var tenantMetrics);

        return Result<AdminTenantResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapTenant(refreshed, tenantMetrics));
    }
}

public sealed class SuspendAdminTenantUseCase(
    ITenantRepository tenantRepository,
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : ISuspendAdminTenantUseCase
{
    public async Task<Result<AdminTenantResponse?>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminTenantResponse?>.Forbidden();

        if (tenantId == TripflowDbSeedData.PlatformTenantId)
            return Result<AdminTenantResponse?>.Failure("Não é permitido alterar o tenant da plataforma.");

        var tenant = await tenantRepository.GetTrackedByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
            return Result<AdminTenantResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        tenant.Suspend(updatedBy);
        await tenantRepository.UpdateAsync(tenant, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            tenantId,
            null,
            AuditActions.TenantSuspended,
            nameof(Tenant),
            tenantId,
            $"Tenant {tenant.TradeName} suspenso."), cancellationToken);

        var refreshed = await platformAdminRepository.GetTenantWithDetailsAsync(tenantId, cancellationToken);
        var now = DateTime.UtcNow;
        var metrics = await platformAdminRepository.GetTenantMetricsAsync([tenantId], now.Year, now.Month, cancellationToken);
        metrics.TryGetValue(tenantId, out var tenantMetrics);

        return Result<AdminTenantResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapTenant(refreshed, tenantMetrics));
    }
}

public sealed class CancelAdminTenantUseCase(
    ITenantRepository tenantRepository,
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : ICancelAdminTenantUseCase
{
    public async Task<Result<AdminTenantResponse?>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminTenantResponse?>.Forbidden();

        if (tenantId == TripflowDbSeedData.PlatformTenantId)
            return Result<AdminTenantResponse?>.Failure("Não é permitido alterar o tenant da plataforma.");

        var tenant = await tenantRepository.GetTrackedByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
            return Result<AdminTenantResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        tenant.Cancel(updatedBy);
        await tenantRepository.UpdateAsync(tenant, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            tenantId,
            null,
            AuditActions.TenantCancelled,
            nameof(Tenant),
            tenantId,
            $"Tenant {tenant.TradeName} cancelado."), cancellationToken);

        var refreshed = await platformAdminRepository.GetTenantWithDetailsAsync(tenantId, cancellationToken);
        var now = DateTime.UtcNow;
        var metrics = await platformAdminRepository.GetTenantMetricsAsync([tenantId], now.Year, now.Month, cancellationToken);
        metrics.TryGetValue(tenantId, out var tenantMetrics);

        return Result<AdminTenantResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapTenant(refreshed, tenantMetrics));
    }
}

public sealed class UpdateAdminTenantUseCase(
    ITenantRepository tenantRepository,
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IValidator<UpdateTenantValidationRequest> validator,
    IUserContext userContext) : IUpdateAdminTenantUseCase
{
    public async Task<Result<AdminTenantResponse?>> ExecuteAsync(
        Guid tenantId,
        UpdateTenantRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminTenantResponse?>.Forbidden();

        if (tenantId == TripflowDbSeedData.PlatformTenantId)
            return Result<AdminTenantResponse?>.Failure("Não é permitido alterar o tenant da plataforma.");

        var validationResult = await validator.ValidateAsync(
            UpdateTenantValidationRequest.From(tenantId, request),
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault();
            return Result<AdminTenantResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var tenant = await tenantRepository.GetTrackedByIdAsync(tenantId, cancellationToken);

        if (tenant is null)
            return Result<AdminTenantResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        tenant.Update(
            request.LegalName,
            request.TradeName,
            request.DocumentNumber,
            request.Email,
            request.Phone,
            request.Status,
            updatedBy);

        await tenantRepository.UpdateAsync(tenant, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            tenantId,
            null,
            AuditActions.TenantUpdated,
            nameof(Tenant),
            tenantId,
            $"Tenant {tenant.TradeName} atualizado."), cancellationToken);

        var refreshed = await platformAdminRepository.GetTenantWithDetailsAsync(tenantId, cancellationToken);
        var now = DateTime.UtcNow;
        var metrics = await platformAdminRepository.GetTenantMetricsAsync([tenantId], now.Year, now.Month, cancellationToken);
        metrics.TryGetValue(tenantId, out var tenantMetrics);

        return Result<AdminTenantResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapTenant(refreshed, tenantMetrics));
    }
}
