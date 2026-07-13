using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Subscriptions;

public sealed class GetCurrentTenantSubscriptionUseCase(
    ITenantSubscriptionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCurrentTenantSubscriptionUseCase
{
    public async Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantSubscriptionResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken))
            return Result<TenantSubscriptionResponse?>.Forbidden();

        var subscription = await repository.GetByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        if (subscription is null)
            return Result<TenantSubscriptionResponse?>.Ok(null);

        return Result<TenantSubscriptionResponse?>.Ok(TenantSubscriptionMapping.Map(subscription));
    }
}

public sealed class UpdateTenantSubscriptionUseCase(
    ITenantSubscriptionRepository repository,
    ISubscriptionPlanRepository subscriptionPlanRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdateTenantSubscriptionRequest> validator,
    ILogger<UpdateTenantSubscriptionUseCase> logger) : IUpdateTenantSubscriptionUseCase
{
    public async Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(UpdateTenantSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantSubscriptionResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantSubscriptionResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<TenantSubscriptionResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var plan = await subscriptionPlanRepository.GetTrackedByIdAsync(request.SubscriptionPlanId, cancellationToken);
        if (plan is null)
            return Result<TenantSubscriptionResponse?>.Failure("Plano não encontrado.");
        if (!plan.CanBeAssigned())
            return Result<TenantSubscriptionResponse?>.Failure("Plano descontinuado não pode ser atribuído.");

        var tenantId = tenantContext.TenantId.Value;
        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        await repository.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);
            if (entity is null)
            {
                entity = new TenantSubscription(
                    tenantId,
                    request.SubscriptionPlanId,
                    request.Status,
                    request.StartedAtUtc,
                    request.ExpiresAtUtc,
                    request.TrialEndsAtUtc,
                    changedBy);

                await repository.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdatePlan(
                    request.SubscriptionPlanId,
                    request.Status,
                    request.StartedAtUtc,
                    request.ExpiresAtUtc,
                    request.TrialEndsAtUtc,
                    changedBy);

                await repository.UpdateAsync(entity, cancellationToken);
            }

            await repository.CommitTransactionAsync(cancellationToken);

            var current = await repository.GetByTenantIdAsync(tenantId, cancellationToken);
            return Result<TenantSubscriptionResponse?>.Ok(current is null ? null : TenantSubscriptionMapping.Map(current));
        }
        catch (Exception ex)
        {
            await repository.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "UpdateTenantSubscriptionUseCase | Erro ao atualizar assinatura.");
            return Result<TenantSubscriptionResponse?>.Failure("Erro inesperado ao atualizar assinatura.");
        }
    }
}

public sealed class ActivateTenantSubscriptionUseCase(
    ITenantSubscriptionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IActivateTenantSubscriptionUseCase
{
    public async Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default)
        => await ChangeStatusAsync(cancellationToken, (s, user) => s.Activate(user));

    private async Task<Result<TenantSubscriptionResponse?>> ChangeStatusAsync(CancellationToken cancellationToken, Action<TenantSubscription, string> change)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantSubscriptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantSubscriptionResponse?>.Forbidden();

        var entity = await repository.GetTrackedByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        if (entity is null)
            return Result<TenantSubscriptionResponse?>.Failure("Assinatura não encontrada.");

        change(entity, userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
        await repository.UpdateAsync(entity, cancellationToken);
        var current = await repository.GetByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        return Result<TenantSubscriptionResponse?>.Ok(current is null ? null : TenantSubscriptionMapping.Map(current));
    }
}

public sealed class SuspendTenantSubscriptionUseCase(
    ITenantSubscriptionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ISuspendTenantSubscriptionUseCase
{
    public async Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantSubscriptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantSubscriptionResponse?>.Forbidden();

        var entity = await repository.GetTrackedByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        if (entity is null)
            return Result<TenantSubscriptionResponse?>.Failure("Assinatura não encontrada.");

        entity.Suspend(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
        await repository.UpdateAsync(entity, cancellationToken);

        var current = await repository.GetByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        return Result<TenantSubscriptionResponse?>.Ok(current is null ? null : TenantSubscriptionMapping.Map(current));
    }
}

public sealed class CancelTenantSubscriptionUseCase(
    ITenantSubscriptionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ICancelTenantSubscriptionUseCase
{
    public async Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantSubscriptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantSubscriptionResponse?>.Forbidden();

        var entity = await repository.GetTrackedByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        if (entity is null)
            return Result<TenantSubscriptionResponse?>.Failure("Assinatura não encontrada.");

        entity.Cancel(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
        await repository.UpdateAsync(entity, cancellationToken);
        var current = await repository.GetByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        return Result<TenantSubscriptionResponse?>.Ok(current is null ? null : TenantSubscriptionMapping.Map(current));
    }
}

public sealed class MarkTenantSubscriptionPastDueUseCase(
    ITenantSubscriptionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IMarkTenantSubscriptionPastDueUseCase
{
    public async Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantSubscriptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantSubscriptionResponse?>.Forbidden();

        var entity = await repository.GetTrackedByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        if (entity is null)
            return Result<TenantSubscriptionResponse?>.Failure("Assinatura não encontrada.");

        entity.MarkPastDue(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
        await repository.UpdateAsync(entity, cancellationToken);
        var current = await repository.GetByTenantIdAsync(tenantContext.TenantId.Value, cancellationToken);
        return Result<TenantSubscriptionResponse?>.Ok(current is null ? null : TenantSubscriptionMapping.Map(current));
    }
}

internal static class TenantSubscriptionMapping
{
    public static TenantSubscriptionResponse Map(TenantSubscription entity)
        => new()
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            SubscriptionPlanId = entity.SubscriptionPlanId,
            SubscriptionPlanName = entity.SubscriptionPlan?.Name ?? string.Empty,
            Status = entity.Status,
            StartedAtUtc = entity.StartedAtUtc,
            ExpiresAtUtc = entity.ExpiresAtUtc,
            TrialEndsAtUtc = entity.TrialEndsAtUtc,
            CancelledAtUtc = entity.CancelledAtUtc,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
}
