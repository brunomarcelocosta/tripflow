using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Leads.Interfaces;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Leads;

public sealed class CreateLeadUseCase(
    ILeadRepository repository,
    IValidator<CreateLeadRequest> validator,
    ILogger<CreateLeadUseCase> logger) : ICreateLeadUseCase
{
    public async Task<Result<LeadResponse?>> ExecuteAsync(CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<LeadResponse?>.Failure(validation.Errors.First().ErrorMessage);

        try
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var lead = new Lead(
                request.CompanyName.Trim(),
                request.ResponsibleName.Trim(),
                normalizedEmail,
                request.Phone?.Trim(),
                request.PlanOfInterest?.Trim(),
                request.Message?.Trim(),
                "public");

            await repository.AddAsync(lead, cancellationToken);
            return Result<LeadResponse?>.Ok(LeadMapping.Map(lead));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CreateLeadUseCase | Erro ao criar lead.");
            return Result<LeadResponse?>.Failure("Erro inesperado ao criar lead.");
        }
    }
}

public sealed class GetLeadsUseCase(
    ILeadRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<LeadFilterRequest> validator) : IGetLeadsUseCase
{
    public async Task<Result<PagedResponse<LeadResponse>>> ExecuteAsync(LeadFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<LeadResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<PagedResponse<LeadResponse>>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<PagedResponse<LeadResponse>>.Failure(validation.Errors.First().ErrorMessage);

        var paged = await repository.GetPagedAsync(
            request.ToExpression(),
            request.Page,
            request.PageSize,
            LeadOrderByHelper.Build(request.SortBy),
            request.SortDesc);

        var response = new PagedResponse<LeadResponse>(
            paged.Items.Select(LeadMapping.Map).ToList(),
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages);

        return Result<PagedResponse<LeadResponse>>.Ok(response);
    }
}

public sealed class GetLeadByIdUseCase(
    ILeadRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IGetLeadByIdUseCase
{
    public async Task<Result<LeadResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<LeadResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<LeadResponse?>.Forbidden();

        var lead = await repository.GetByIdAsync(id);
        return Result<LeadResponse?>.Ok(lead is null ? null : LeadMapping.Map(lead));
    }
}

public sealed class ConvertLeadToTenantUseCase(
    ILeadRepository leadRepository,
    ITenantRepository tenantRepository,
    ITenantBrandingRepository tenantBrandingRepository,
    ITenantCommercialSettingsRepository tenantCommercialSettingsRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ISubscriptionPlanRepository subscriptionPlanRepository,
    ITenantRoleProvisioningService tenantRoleProvisioningService,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<ConvertLeadToTenantRequest> validator,
    ILogger<ConvertLeadToTenantUseCase> logger) : IConvertLeadToTenantUseCase
{
    public async Task<Result<Guid?>> ExecuteAsync(Guid leadId, ConvertLeadToTenantRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<Guid?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<Guid?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<Guid?>.Failure(validation.Errors.First().ErrorMessage);

        var lead = await leadRepository.GetTrackedByIdAsync(leadId, cancellationToken);
        if (lead is null)
            return Result<Guid?>.Failure("Lead não encontrado.");
        if (lead.ConvertedToTenant)
            return Result<Guid?>.Failure("Este lead já foi convertido.");

        var plan = default(Domain.Entities.Subscriptions.SubscriptionPlan);
        if (request.SubscriptionPlanId.HasValue)
        {
            plan = await subscriptionPlanRepository.GetTrackedByIdAsync(request.SubscriptionPlanId.Value, cancellationToken);
            if (plan is null)
                return Result<Guid?>.Failure("Plano de assinatura não encontrado.");
            if (!plan.CanBeAssigned())
                return Result<Guid?>.Failure("Plano de assinatura descontinuado não pode ser atribuído.");
        }

        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        await tenantRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            var tenant = new Tenant(
                request.LegalName.Trim(),
                request.TradeName.Trim(),
                documentNumber: request.DocumentNumber?.Trim(),
                email: lead.Email,
                phone: lead.Phone,
                status: TenantStatus.Active,
                createdBy: changedBy);

            await tenantRepository.AddAsync(tenant, cancellationToken);
            await tenantRoleProvisioningService.ProvisionDefaultRolesAsync(tenant.Id, cancellationToken);

            var branding = new TenantBranding(
                tenant.Id,
                logoUrl: null,
                primaryColor: null,
                secondaryColor: null,
                textColor: null,
                proposalFooter: null,
                createdBy: changedBy);

            await tenantBrandingRepository.AddAsync(branding, cancellationToken);

            var commercialSettings = new TenantCommercialSettings(tenant.Id, changedBy);
            await tenantCommercialSettingsRepository.AddAsync(commercialSettings, cancellationToken);

            if (plan is not null)
            {
                var subscription = new TenantSubscription(
                    tenant.Id,
                    plan.Id,
                    TenantSubscriptionStatus.Active,
                    DateTime.UtcNow,
                    expiresAtUtc: null,
                    trialEndsAtUtc: null,
                    createdBy: changedBy);

                await tenantSubscriptionRepository.AddAsync(subscription, cancellationToken);
            }

            lead.MarkAsConverted(tenant.Id, changedBy);
            await leadRepository.UpdateAsync(lead, cancellationToken);

            await tenantRepository.CommitTransactionAsync(cancellationToken);
            return Result<Guid?>.Ok(tenant.Id);
        }
        catch (Exception ex)
        {
            await tenantRepository.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "ConvertLeadToTenantUseCase | Erro ao converter lead.");
            return Result<Guid?>.Failure("Erro inesperado ao converter lead em tenant.");
        }
    }
}

public sealed class DeleteLeadUseCase(
    ILeadRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteLeadUseCase> logger) : IDeleteLeadUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid leadId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<bool>.Forbidden();

        var lead = await repository.GetTrackedByIdAsync(leadId, cancellationToken);
        if (lead is null)
            return Result<bool>.Failure("Lead não encontrado.");

        try
        {
            lead.SetDelete(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
            await repository.UpdateAsync(lead, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteLeadUseCase | Erro ao remover lead.");
            return Result<bool>.Failure("Erro inesperado ao remover lead.");
        }
    }
}

internal static class LeadMapping
{
    public static LeadResponse Map(Lead lead)
        => new()
        {
            Id = lead.Id,
            CompanyName = lead.CompanyName,
            ResponsibleName = lead.ResponsibleName,
            Email = lead.Email,
            Phone = lead.Phone,
            PlanOfInterest = lead.PlanOfInterest,
            Message = lead.Message,
            ConvertedToTenant = lead.ConvertedToTenant,
            ConvertedTenantId = lead.ConvertedTenantId,
            CreatedAtUtc = lead.CreatedAtUtc,
            UpdatedAtUtc = lead.UpdatedAtUtc
        };
}
